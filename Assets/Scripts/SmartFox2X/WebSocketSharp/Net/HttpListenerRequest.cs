using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerRequest
	{
		private static readonly byte[] _100continue;

		private string[] _acceptTypes;

		private bool _chunked;

		private Encoding _contentEncoding;

		private long _contentLength;

		private bool _contentLengthWasSet;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private WebHeaderCollection _headers;

		private Guid _identifier;

		private Stream _inputStream;

		private bool _keepAlive;

		private bool _keepAliveWasSet;

		private string _method;

		private NameValueCollection _queryString;

		private Uri _referer;

		private string _uri;

		private Uri _url;

		private string[] _userLanguages;

		private Version _version;

		private bool _websocketRequest;

		private bool _websocketRequestWasSet;

		public string[] AcceptTypes
		{
			get
			{
				return _acceptTypes;
			}
		}

		public int ClientCertificateError
		{
			get
			{
				return 0;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				return _contentEncoding ?? (_contentEncoding = Encoding.Default);
			}
		}

		public long ContentLength64
		{
			get
			{
				return _contentLength;
			}
		}

		public string ContentType
		{
			get
			{
				return _headers["Content-Type"];
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				return _cookies ?? (_cookies = _headers.GetCookies(false));
			}
		}

		public bool HasEntityBody
		{
			get
			{
				return _contentLength > 0 || _chunked;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		public string HttpMethod
		{
			get
			{
				return _method;
			}
		}

		public Stream InputStream
		{
			get
			{
				return _inputStream ?? (_inputStream = ((!HasEntityBody) ? Stream.Null : _context.Connection.GetRequestStream(_contentLength, _chunked)));
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return _context.User != null;
			}
		}

		public bool IsLocal
		{
			get
			{
				return RemoteEndPoint.Address.IsLocal();
			}
		}

		public bool IsSecureConnection
		{
			get
			{
				return _context.Connection.IsSecure;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				if (!_websocketRequestWasSet)
				{
					_websocketRequest = _method == "GET" && _version > HttpVersion.Version10 && _headers.Contains("Upgrade", "websocket") && _headers.Contains("Connection", "Upgrade");
					_websocketRequestWasSet = true;
				}
				return _websocketRequest;
			}
		}

		public bool KeepAlive
		{
			get
			{
				if (!_keepAliveWasSet)
				{
					string text;
					_keepAlive = _version > HttpVersion.Version10 || _headers.Contains("Connection", "keep-alive") || ((text = _headers["Keep-Alive"]) != null && text != "closed");
					_keepAliveWasSet = true;
				}
				return _keepAlive;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return _context.Connection.LocalEndPoint;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return _version;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				return _queryString ?? (_queryString = HttpUtility.InternalParseQueryString(_url.Query, Encoding.UTF8));
			}
		}

		public string RawUrl
		{
			get
			{
				return _url.PathAndQuery;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return _context.Connection.RemoteEndPoint;
			}
		}

		public Guid RequestTraceIdentifier
		{
			get
			{
				return _identifier;
			}
		}

		public Uri Url
		{
			get
			{
				return _url;
			}
		}

		public Uri UrlReferrer
		{
			get
			{
				return _referer;
			}
		}

		public string UserAgent
		{
			get
			{
				return _headers["User-Agent"];
			}
		}

		public string UserHostAddress
		{
			get
			{
				return LocalEndPoint.ToString();
			}
		}

		public string UserHostName
		{
			get
			{
				return _headers["Host"];
			}
		}

		public string[] UserLanguages
		{
			get
			{
				return _userLanguages;
			}
		}

		internal HttpListenerRequest(HttpListenerContext context)
		{
			_context = context;
			_contentLength = -1L;
			_headers = new WebHeaderCollection();
			_identifier = Guid.NewGuid();
		}

		static HttpListenerRequest()
		{
			_100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");
		}

		private static bool tryCreateVersion(string version, out Version result)
		{
			try
			{
				result = new Version(version);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		internal void AddHeader(string header)
		{
			int num = header.IndexOf(':');
			if (num == -1)
			{
				_context.ErrorMessage = "Invalid header";
				return;
			}
			string text = header.Substring(0, num).Trim();
			string text2 = header.Substring(num + 1).Trim();
			_headers.InternalSet(text, text2, false);
			switch (text.ToLower(CultureInfo.InvariantCulture))
			{
			case "accept":
				_acceptTypes = new List<string>(text2.SplitHeaderValue(',')).ToArray();
				break;
			case "accept-language":
				_userLanguages = text2.Split(',');
				break;
			case "content-length":
			{
				long result;
				if (long.TryParse(text2, out result) && result >= 0)
				{
					_contentLength = result;
					_contentLengthWasSet = true;
				}
				else
				{
					_context.ErrorMessage = "Invalid Content-Length header";
				}
				break;
			}
			case "content-type":
				try
				{
					_contentEncoding = HttpUtility.GetEncoding(text2);
					break;
				}
				catch
				{
					_context.ErrorMessage = "Invalid Content-Type header";
					break;
				}
			case "referer":
				_referer = text2.ToUri();
				break;
			}
		}

		internal void FinishInitialization()
		{
			string text = _headers["Host"];
			bool flag = text == null || text.Length == 0;
			if (_version > HttpVersion.Version10 && flag)
			{
				_context.ErrorMessage = "Invalid Host header";
				return;
			}
			if (flag)
			{
				text = UserHostAddress;
			}
			_url = HttpUtility.CreateRequestUrl(_uri, text, IsWebSocketRequest, IsSecureConnection);
			if (_url == null)
			{
				_context.ErrorMessage = "Invalid request url";
				return;
			}
			string text2 = Headers["Transfer-Encoding"];
			if (_version > HttpVersion.Version10 && text2 != null && text2.Length > 0)
			{
				_chunked = text2.ToLower() == "chunked";
				if (!_chunked)
				{
					_context.ErrorMessage = string.Empty;
					_context.ErrorStatus = 501;
					return;
				}
			}
			if (!_chunked && !_contentLengthWasSet)
			{
				string text3 = _method.ToLower();
				if (text3 == "post" || text3 == "put")
				{
					_context.ErrorMessage = string.Empty;
					_context.ErrorStatus = 411;
					return;
				}
			}
			string text4 = Headers["Expect"];
			if (text4 != null && text4.Length > 0 && text4.ToLower() == "100-continue")
			{
				ResponseStream responseStream = _context.Connection.GetResponseStream();
				responseStream.WriteInternally(_100continue, 0, _100continue.Length);
			}
		}

		internal bool FlushInput()
		{
			if (!HasEntityBody)
			{
				return true;
			}
			int num = 2048;
			if (_contentLength > 0)
			{
				num = (int)Math.Min(_contentLength, num);
			}
			byte[] buffer = new byte[num];
			while (true)
			{
				try
				{
					IAsyncResult asyncResult = InputStream.BeginRead(buffer, 0, num, null, null);
					if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(100))
					{
						return false;
					}
					if (InputStream.EndRead(asyncResult) <= 0)
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		internal void SetRequestLine(string requestLine)
		{
			string[] array = requestLine.Split(new char[1] { ' ' }, 3);
			if (array.Length != 3)
			{
				_context.ErrorMessage = "Invalid request line (parts)";
				return;
			}
			_method = array[0];
			if (!_method.IsToken())
			{
				_context.ErrorMessage = "Invalid request line (method)";
				return;
			}
			_uri = array[1];
			string text = array[2];
			if (text.Length != 8 || !text.StartsWith("HTTP/") || !tryCreateVersion(text.Substring(5), out _version) || _version.Major < 1)
			{
				_context.ErrorMessage = "Invalid request line (version)";
			}
		}

		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			throw new NotImplementedException();
		}

		public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public X509Certificate2 GetClientCertificate()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}\r\n", _method, _uri, _version);
			stringBuilder.Append(_headers.ToString());
			return stringBuilder.ToString();
		}
	}
}
