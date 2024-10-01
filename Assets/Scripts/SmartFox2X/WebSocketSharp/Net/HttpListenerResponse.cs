using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerResponse : IDisposable
	{
		private bool _chunked;

		private Encoding _contentEncoding;

		private long _contentLength;

		private bool _contentLengthWasSet;

		private string _contentType;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private bool _disposed;

		private bool _forceCloseChunked;

		private WebHeaderCollection _headers;

		private bool _headersWereSent;

		private bool _keepAlive;

		private string _location;

		private ResponseStream _outputStream;

		private int _statusCode;

		private string _statusDescription;

		private Version _version;

		internal bool CloseConnection
		{
			get
			{
				return _headers["Connection"] == "close";
			}
		}

		internal bool ForceCloseChunked
		{
			get
			{
				return _forceCloseChunked;
			}
		}

		internal bool HeadersSent
		{
			get
			{
				return _headersWereSent;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				return _contentEncoding;
			}
			set
			{
				checkDisposedOrHeadersSent();
				_contentEncoding = value;
			}
		}

		public long ContentLength64
		{
			get
			{
				return _contentLength;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Less than zero.", "value");
				}
				_contentLengthWasSet = true;
				_contentLength = value;
			}
		}

		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("An empty string.", "value");
				}
				_contentType = value;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				return _cookies ?? (_cookies = new CookieCollection());
			}
			set
			{
				checkDisposedOrHeadersSent();
				_cookies = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return _headers;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_headers = value;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return _keepAlive;
			}
			set
			{
				checkDisposedOrHeadersSent();
				_keepAlive = value;
			}
		}

		public Stream OutputStream
		{
			get
			{
				checkDisposed();
				return _outputStream ?? (_outputStream = _context.Connection.GetResponseStream());
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return _version;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Major != 1 || (value.Minor != 0 && value.Minor != 1))
				{
					throw new ArgumentException("Not 1.0 or 1.1.", "value");
				}
				_version = value;
			}
		}

		public string RedirectLocation
		{
			get
			{
				return _location;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value.Length == 0)
				{
					throw new ArgumentException("An empty string.", "value");
				}
				_location = value;
			}
		}

		public bool SendChunked
		{
			get
			{
				return _chunked;
			}
			set
			{
				checkDisposedOrHeadersSent();
				_chunked = value;
			}
		}

		public int StatusCode
		{
			get
			{
				return _statusCode;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value < 100 || value > 999)
				{
					throw new ProtocolViolationException("A value isn't between 100 and 999.");
				}
				_statusCode = value;
				_statusDescription = value.GetStatusDescription();
			}
		}

		public string StatusDescription
		{
			get
			{
				return _statusDescription;
			}
			set
			{
				checkDisposedOrHeadersSent();
				_statusDescription = ((value == null || value.Length <= 0) ? _statusCode.GetStatusDescription() : value);
			}
		}

		internal HttpListenerResponse(HttpListenerContext context)
		{
			_context = context;
			_headers = new WebHeaderCollection();
			_keepAlive = true;
			_statusCode = 200;
			_statusDescription = "OK";
			_version = HttpVersion.Version11;
		}

		void IDisposable.Dispose()
		{
			if (!_disposed)
			{
				close(true);
			}
		}

		private bool canAddOrUpdate(Cookie cookie)
		{
			if (_cookies == null || _cookies.Count == 0)
			{
				return true;
			}
			List<Cookie> list = findCookie(cookie).ToList();
			if (list.Count == 0)
			{
				return true;
			}
			int version = cookie.Version;
			foreach (Cookie item in list)
			{
				if (item.Version == version)
				{
					return true;
				}
			}
			return false;
		}

		private void checkDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}

		private void checkDisposedOrHeadersSent()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (_headersWereSent)
			{
				throw new InvalidOperationException("Cannot be changed after the headers are sent.");
			}
		}

		private void close(bool force)
		{
			_disposed = true;
			_context.Connection.Close(force);
		}

		private IEnumerable<Cookie> findCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string domain = cookie.Domain;
			string path = cookie.Path;
			if (_cookies == null)
			{
				yield break;
			}
			IEnumerator enumerator = _cookies.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Cookie c = (Cookie)enumerator.Current;
					if (c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) && c.Path.Equals(path, StringComparison.Ordinal))
					{
						yield return c;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable2 = (disposable = enumerator as IDisposable);
				if (disposable != null)
				{
					disposable2.Dispose();
				}
			}
		}

		internal void SendHeaders(MemoryStream stream, bool closing)
		{
			if (_contentType != null)
			{
				string value = ((_contentType.IndexOf("charset=", StringComparison.Ordinal) != -1 || _contentEncoding == null) ? _contentType : string.Format("{0}; charset={1}", _contentType, _contentEncoding.WebName));
				_headers.InternalSet("Content-Type", value, true);
			}
			if (_headers["Server"] == null)
			{
				_headers.InternalSet("Server", "websocket-sharp/1.0", true);
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (_headers["Date"] == null)
			{
				_headers.InternalSet("Date", DateTime.UtcNow.ToString("r", invariantCulture), true);
			}
			if (!_chunked)
			{
				if (!_contentLengthWasSet && closing)
				{
					_contentLengthWasSet = true;
					_contentLength = 0L;
				}
				if (_contentLengthWasSet)
				{
					_headers.InternalSet("Content-Length", _contentLength.ToString(invariantCulture), true);
				}
			}
			Version protocolVersion = _context.Request.ProtocolVersion;
			if (!_contentLengthWasSet && !_chunked && protocolVersion > HttpVersion.Version10)
			{
				_chunked = true;
			}
			bool flag = _statusCode == 400 || _statusCode == 408 || _statusCode == 411 || _statusCode == 413 || _statusCode == 414 || _statusCode == 500 || _statusCode == 503;
			if (!flag)
			{
				flag = !_context.Request.KeepAlive;
			}
			if (!_keepAlive || flag)
			{
				_headers.InternalSet("Connection", "close", true);
				flag = true;
			}
			if (_chunked)
			{
				_headers.InternalSet("Transfer-Encoding", "chunked", true);
			}
			int reuses = _context.Connection.Reuses;
			if (reuses >= 100)
			{
				_forceCloseChunked = true;
				if (!flag)
				{
					_headers.InternalSet("Connection", "close", true);
					flag = true;
				}
			}
			if (!flag)
			{
				_headers.InternalSet("Keep-Alive", string.Format("timeout=15,max={0}", 100 - reuses), true);
				if (protocolVersion < HttpVersion.Version11)
				{
					_headers.InternalSet("Connection", "keep-alive", true);
				}
			}
			if (_location != null)
			{
				_headers.InternalSet("Location", _location, true);
			}
			if (_cookies != null)
			{
				foreach (Cookie cookie in _cookies)
				{
					_headers.InternalSet("Set-Cookie", cookie.ToResponseString(), true);
				}
			}
			Encoding encoding = _contentEncoding ?? Encoding.Default;
			StreamWriter streamWriter = new StreamWriter(stream, encoding, 256);
			streamWriter.Write("HTTP/{0} {1} {2}\r\n", _version, _statusCode, _statusDescription);
			streamWriter.Write(_headers.ToStringMultiValue(true));
			streamWriter.Flush();
			stream.Position = ((encoding.CodePage != 65001) ? encoding.GetPreamble().Length : 3);
			if (_outputStream == null)
			{
				_outputStream = _context.Connection.GetResponseStream();
			}
			_headersWereSent = true;
		}

		public void Abort()
		{
			if (!_disposed)
			{
				close(true);
			}
		}

		public void AddHeader(string name, string value)
		{
			checkDisposedOrHeadersSent();
			_headers.Set(name, value);
		}

		public void AppendCookie(Cookie cookie)
		{
			checkDisposedOrHeadersSent();
			Cookies.Add(cookie);
		}

		public void AppendHeader(string name, string value)
		{
			checkDisposedOrHeadersSent();
			_headers.Add(name, value);
		}

		public void Close()
		{
			if (!_disposed)
			{
				close(false);
			}
		}

		public void Close(byte[] responseEntity, bool willBlock)
		{
			if (responseEntity == null)
			{
				throw new ArgumentNullException("responseEntity");
			}
			int num = responseEntity.Length;
			ContentLength64 = num;
			Stream output = OutputStream;
			if (willBlock)
			{
				output.Write(responseEntity, 0, num);
				close(false);
				return;
			}
			output.BeginWrite(responseEntity, 0, num, delegate(IAsyncResult ar)
			{
				output.EndWrite(ar);
				close(false);
			}, null);
		}

		public void CopyFrom(HttpListenerResponse templateResponse)
		{
			checkDisposedOrHeadersSent();
			_headers.Clear();
			_headers.Add(templateResponse._headers);
			_contentLength = templateResponse._contentLength;
			_statusCode = templateResponse._statusCode;
			_statusDescription = templateResponse._statusDescription;
			_keepAlive = templateResponse._keepAlive;
			_version = templateResponse._version;
		}

		public void Redirect(string url)
		{
			StatusCode = 302;
			_location = url;
		}

		public void SetCookie(Cookie cookie)
		{
			checkDisposedOrHeadersSent();
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (!canAddOrUpdate(cookie))
			{
				throw new ArgumentException("Cannot be replaced.", "cookie");
			}
			Cookies.Add(cookie);
		}
	}
}
