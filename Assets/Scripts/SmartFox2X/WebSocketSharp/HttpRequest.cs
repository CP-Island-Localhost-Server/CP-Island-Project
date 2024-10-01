using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal class HttpRequest : HttpBase
	{
		private string _method;

		private string _uri;

		private bool _websocketRequest;

		private bool _websocketRequestWasSet;

		public AuthenticationResponse AuthenticationResponse
		{
			get
			{
				string text = base.Headers["Authorization"];
				return (text == null || text.Length <= 0) ? null : AuthenticationResponse.Parse(text);
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(false);
			}
		}

		public string HttpMethod
		{
			get
			{
				return _method;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				if (!_websocketRequestWasSet)
				{
					NameValueCollection headers = base.Headers;
					_websocketRequest = _method == "GET" && base.ProtocolVersion > HttpVersion.Version10 && headers.Contains("Upgrade", "websocket") && headers.Contains("Connection", "Upgrade");
					_websocketRequestWasSet = true;
				}
				return _websocketRequest;
			}
		}

		public string RequestUri
		{
			get
			{
				return _uri;
			}
		}

		private HttpRequest(string method, string uri, Version version, NameValueCollection headers)
			: base(version, headers)
		{
			_method = method;
			_uri = uri;
		}

		internal HttpRequest(string method, string uri)
			: this(method, uri, HttpVersion.Version11, new NameValueCollection())
		{
			base.Headers["User-Agent"] = "websocket-sharp/1.0";
		}

		internal static HttpRequest CreateConnectRequest(Uri uri)
		{
			string dnsSafeHost = uri.DnsSafeHost;
			int port = uri.Port;
			string text = string.Format("{0}:{1}", dnsSafeHost, port);
			HttpRequest httpRequest = new HttpRequest("CONNECT", text);
			httpRequest.Headers["Host"] = ((port != 80) ? text : dnsSafeHost);
			return httpRequest;
		}

		internal static HttpRequest CreateWebSocketRequest(Uri uri)
		{
			HttpRequest httpRequest = new HttpRequest("GET", uri.PathAndQuery);
			NameValueCollection headers = httpRequest.Headers;
			headers["Upgrade"] = "websocket";
			headers["Connection"] = "Upgrade";
			headers["Host"] = ((uri.Port != 80) ? uri.Authority : uri.DnsSafeHost);
			return httpRequest;
		}

		internal HttpResponse GetResponse(Stream stream, int millisecondsTimeout)
		{
			byte[] array = ToByteArray();
			stream.Write(array, 0, array.Length);
			return HttpBase.Read(stream, HttpResponse.Parse, millisecondsTimeout);
		}

		internal static HttpRequest Parse(string[] headerParts)
		{
			string[] array = headerParts[0].Split(new char[1] { ' ' }, 3);
			if (array.Length != 3)
			{
				throw new ArgumentException("Invalid request line: " + headerParts[0]);
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < headerParts.Length; i++)
			{
				webHeaderCollection.InternalSet(headerParts[i], false);
			}
			return new HttpRequest(array[0], array[1], new Version(array[2].Substring(5)), webHeaderCollection);
		}

		internal static HttpRequest Read(Stream stream, int millisecondsTimeout)
		{
			return HttpBase.Read(stream, Parse, millisecondsTimeout);
		}

		public void SetCookies(CookieCollection cookies)
		{
			if (cookies == null || cookies.Count == 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			foreach (Cookie item in cookies.Sorted)
			{
				if (!item.Expired)
				{
					stringBuilder.AppendFormat("{0}; ", item.ToString());
				}
			}
			int length = stringBuilder.Length;
			if (length > 2)
			{
				stringBuilder.Length = length - 2;
				base.Headers["Cookie"] = stringBuilder.ToString();
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}{3}", _method, _uri, base.ProtocolVersion, "\r\n");
			NameValueCollection headers = base.Headers;
			string[] allKeys = headers.AllKeys;
			foreach (string text in allKeys)
			{
				stringBuilder.AppendFormat("{0}: {1}{2}", text, headers[text], "\r\n");
			}
			stringBuilder.Append("\r\n");
			string entityBody = base.EntityBody;
			if (entityBody.Length > 0)
			{
				stringBuilder.Append(entityBody);
			}
			return stringBuilder.ToString();
		}
	}
}
