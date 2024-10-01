using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal class HttpResponse : HttpBase
	{
		private string _code;

		private string _reason;

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(true);
			}
		}

		public bool HasConnectionClose
		{
			get
			{
				return base.Headers.Contains("Connection", "close");
			}
		}

		public bool IsProxyAuthenticationRequired
		{
			get
			{
				return _code == "407";
			}
		}

		public bool IsRedirect
		{
			get
			{
				return _code == "301" || _code == "302";
			}
		}

		public bool IsUnauthorized
		{
			get
			{
				return _code == "401";
			}
		}

		public bool IsWebSocketResponse
		{
			get
			{
				NameValueCollection headers = base.Headers;
				return base.ProtocolVersion > HttpVersion.Version10 && _code == "101" && headers.Contains("Upgrade", "websocket") && headers.Contains("Connection", "Upgrade");
			}
		}

		public string Reason
		{
			get
			{
				return _reason;
			}
		}

		public string StatusCode
		{
			get
			{
				return _code;
			}
		}

		private HttpResponse(string code, string reason, Version version, NameValueCollection headers)
			: base(version, headers)
		{
			_code = code;
			_reason = reason;
		}

		internal HttpResponse(HttpStatusCode code)
			: this(code, code.GetDescription())
		{
		}

		internal HttpResponse(HttpStatusCode code, string reason)
			: this(((int)code).ToString(), reason, HttpVersion.Version11, new NameValueCollection())
		{
			base.Headers["Server"] = "websocket-sharp/1.0";
		}

		internal static HttpResponse CreateCloseResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = new HttpResponse(code);
			httpResponse.Headers["Connection"] = "close";
			return httpResponse;
		}

		internal static HttpResponse CreateUnauthorizedResponse(string challenge)
		{
			HttpResponse httpResponse = new HttpResponse(HttpStatusCode.Unauthorized);
			httpResponse.Headers["WWW-Authenticate"] = challenge;
			return httpResponse;
		}

		internal static HttpResponse CreateWebSocketResponse()
		{
			HttpResponse httpResponse = new HttpResponse(HttpStatusCode.SwitchingProtocols);
			NameValueCollection headers = httpResponse.Headers;
			headers["Upgrade"] = "websocket";
			headers["Connection"] = "Upgrade";
			return httpResponse;
		}

		internal static HttpResponse Parse(string[] headerParts)
		{
			string[] array = headerParts[0].Split(new char[1] { ' ' }, 3);
			if (array.Length != 3)
			{
				throw new ArgumentException("Invalid status line: " + headerParts[0]);
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < headerParts.Length; i++)
			{
				webHeaderCollection.InternalSet(headerParts[i], true);
			}
			return new HttpResponse(array[1], array[2], new Version(array[0].Substring(5)), webHeaderCollection);
		}

		internal static HttpResponse Read(Stream stream, int millisecondsTimeout)
		{
			return HttpBase.Read(stream, Parse, millisecondsTimeout);
		}

		public void SetCookies(CookieCollection cookies)
		{
			if (cookies == null || cookies.Count == 0)
			{
				return;
			}
			NameValueCollection headers = base.Headers;
			foreach (Cookie item in cookies.Sorted)
			{
				headers.Add("Set-Cookie", item.ToResponseString());
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("HTTP/{0} {1} {2}{3}", base.ProtocolVersion, _code, _reason, "\r\n");
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
