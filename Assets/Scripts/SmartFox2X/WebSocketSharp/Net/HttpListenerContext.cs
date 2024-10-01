using System;
using System.Security.Principal;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerContext
	{
		private HttpConnection _connection;

		private string _error;

		private int _errorStatus;

		private HttpListener _listener;

		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		private IPrincipal _user;

		internal HttpConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}

		internal int ErrorStatus
		{
			get
			{
				return _errorStatus;
			}
			set
			{
				_errorStatus = value;
			}
		}

		internal bool HasError
		{
			get
			{
				return _error != null;
			}
		}

		internal HttpListener Listener
		{
			get
			{
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return _request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return _response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return _user;
			}
			internal set
			{
				_user = value;
			}
		}

		internal HttpListenerContext(HttpConnection connection)
		{
			_connection = connection;
			_errorStatus = 400;
			_request = new HttpListenerRequest(this);
			_response = new HttpListenerResponse(this);
		}

		public HttpListenerWebSocketContext AcceptWebSocket(string protocol)
		{
			if (protocol != null)
			{
				if (protocol.Length == 0)
				{
					throw new ArgumentException("An empty string.", "protocol");
				}
				if (!protocol.IsToken())
				{
					throw new ArgumentException("Contains an invalid character.", "protocol");
				}
			}
			return new HttpListenerWebSocketContext(this, protocol);
		}
	}
}
