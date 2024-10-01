using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace WebSocketSharp.Net.WebSockets
{
	internal class TcpListenerWebSocketContext : WebSocketContext
	{
		private CookieCollection _cookies;

		private Logger _logger;

		private NameValueCollection _queryString;

		private HttpRequest _request;

		private bool _secure;

		private Stream _stream;

		private TcpClient _tcpClient;

		private Uri _uri;

		private IPrincipal _user;

		private WebSocket _websocket;

		internal string HttpMethod
		{
			get
			{
				return _request.HttpMethod;
			}
		}

		internal Logger Log
		{
			get
			{
				return _logger;
			}
		}

		internal Stream Stream
		{
			get
			{
				return _stream;
			}
		}

		public override CookieCollection CookieCollection
		{
			get
			{
				return _cookies ?? (_cookies = _request.Cookies);
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return _request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return _request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return _user != null;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return UserEndPoint.Address.IsLocal();
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return _secure;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return _request.IsWebSocketRequest;
			}
		}

		public override string Origin
		{
			get
			{
				return _request.Headers["Origin"];
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _queryString ?? (_queryString = HttpUtility.InternalParseQueryString((!(_uri != null)) ? null : _uri.Query, Encoding.UTF8));
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return _uri;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return _request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string protocols = _request.Headers["Sec-WebSocket-Protocol"];
				if (protocols != null)
				{
					string[] array = protocols.Split(',');
					foreach (string protocol in array)
					{
						yield return protocol.Trim();
					}
				}
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return _request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return (IPEndPoint)_tcpClient.Client.LocalEndPoint;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return _user;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return (IPEndPoint)_tcpClient.Client.RemoteEndPoint;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return _websocket;
			}
		}

		internal TcpListenerWebSocketContext(TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger logger)
		{
			_tcpClient = tcpClient;
			_secure = secure;
			_logger = logger;
			NetworkStream stream = tcpClient.GetStream();
			if (secure)
			{
				SslStream sslStream = new SslStream(stream, false, sslConfig.ClientCertificateValidationCallback);
				sslStream.AuthenticateAsServer(sslConfig.ServerCertificate, sslConfig.ClientCertificateRequired, sslConfig.EnabledSslProtocols, sslConfig.CheckCertificateRevocation);
				_stream = sslStream;
			}
			else
			{
				_stream = stream;
			}
			_request = HttpRequest.Read(_stream, 90000);
			_uri = HttpUtility.CreateRequestUrl(_request.RequestUri, _request.Headers["Host"], _request.IsWebSocketRequest, secure);
			_websocket = new WebSocket(this, protocol);
		}

		internal void Close()
		{
			_stream.Close();
			_tcpClient.Close();
		}

		internal void Close(HttpStatusCode code)
		{
			_websocket.Close(HttpResponse.CreateCloseResponse(code));
		}

		internal void SendAuthenticationChallenge(string challenge)
		{
			byte[] array = HttpResponse.CreateUnauthorizedResponse(challenge).ToByteArray();
			_stream.Write(array, 0, array.Length);
			_request = HttpRequest.Read(_stream, 15000);
		}

		internal void SetUser(IPrincipal value)
		{
			_user = value;
		}

		public override string ToString()
		{
			return _request.ToString();
		}
	}
}
