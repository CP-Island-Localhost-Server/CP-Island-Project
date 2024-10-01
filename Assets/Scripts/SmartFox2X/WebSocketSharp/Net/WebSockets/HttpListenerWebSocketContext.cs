using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Principal;

namespace WebSocketSharp.Net.WebSockets
{
	public class HttpListenerWebSocketContext : WebSocketContext
	{
		private HttpListenerContext _context;

		private WebSocket _websocket;

		internal Logger Log
		{
			get
			{
				return _context.Listener.Log;
			}
		}

		internal Stream Stream
		{
			get
			{
				return _context.Connection.Stream;
			}
		}

		public override CookieCollection CookieCollection
		{
			get
			{
				return _context.Request.Cookies;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return _context.Request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return _context.Request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return _context.User != null;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return _context.Request.IsLocal;
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return _context.Connection.IsSecure;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return _context.Request.IsWebSocketRequest;
			}
		}

		public override string Origin
		{
			get
			{
				return _context.Request.Headers["Origin"];
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return _context.Request.QueryString;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return _context.Request.Url;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return _context.Request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string protocols = _context.Request.Headers["Sec-WebSocket-Protocol"];
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
				return _context.Request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return _context.Connection.LocalEndPoint;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return _context.User;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return _context.Connection.RemoteEndPoint;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return _websocket;
			}
		}

		internal HttpListenerWebSocketContext(HttpListenerContext context, string protocol)
		{
			_context = context;
			_websocket = new WebSocket(this, protocol);
		}

		internal void Close()
		{
			_context.Connection.Close(true);
		}

		internal void Close(HttpStatusCode code)
		{
			_context.Response.Close(code);
		}

		public override string ToString()
		{
			return _context.Request.ToString();
		}
	}
}
