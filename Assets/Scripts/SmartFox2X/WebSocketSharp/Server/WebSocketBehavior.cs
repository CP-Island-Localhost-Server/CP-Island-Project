using System;
using System.IO;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public abstract class WebSocketBehavior : IWebSocketSession
	{
		private WebSocketContext _context;

		private Func<CookieCollection, CookieCollection, bool> _cookiesValidator;

		private string _id;

		private bool _ignoreExtensions;

		private Func<string, bool> _originValidator;

		private string _protocol;

		private WebSocketSessionManager _sessions;

		private DateTime _startTime;

		private WebSocket _websocket;

		protected Logger Log
		{
			get
			{
				return (_websocket == null) ? null : _websocket.Log;
			}
		}

		protected WebSocketSessionManager Sessions
		{
			get
			{
				return _sessions;
			}
		}

		public WebSocketContext Context
		{
			get
			{
				return _context;
			}
		}

		public Func<CookieCollection, CookieCollection, bool> CookiesValidator
		{
			get
			{
				return _cookiesValidator;
			}
			set
			{
				_cookiesValidator = value;
			}
		}

		public string ID
		{
			get
			{
				return _id;
			}
		}

		public bool IgnoreExtensions
		{
			get
			{
				return _ignoreExtensions;
			}
			set
			{
				_ignoreExtensions = value;
			}
		}

		public Func<string, bool> OriginValidator
		{
			get
			{
				return _originValidator;
			}
			set
			{
				_originValidator = value;
			}
		}

		public string Protocol
		{
			get
			{
				return (_websocket == null) ? (_protocol ?? string.Empty) : _websocket.Protocol;
			}
			set
			{
				if (State == WebSocketState.Connecting && (value == null || (value.Length != 0 && value.IsToken())))
				{
					_protocol = value;
				}
			}
		}

		public DateTime StartTime
		{
			get
			{
				return _startTime;
			}
		}

		public WebSocketState State
		{
			get
			{
				return (_websocket != null) ? _websocket.ReadyState : WebSocketState.Connecting;
			}
		}

		protected WebSocketBehavior()
		{
			_startTime = DateTime.MaxValue;
		}

		private string checkIfValidConnectionRequest(WebSocketContext context)
		{
			return (_originValidator != null && !_originValidator(context.Origin)) ? "Invalid Origin header." : ((_cookiesValidator == null || _cookiesValidator(context.CookieCollection, context.WebSocket.CookieCollection)) ? null : "Invalid Cookies.");
		}

		private void onClose(object sender, CloseEventArgs e)
		{
			if (_id != null)
			{
				_sessions.Remove(_id);
				OnClose(e);
			}
		}

		private void onError(object sender, ErrorEventArgs e)
		{
			OnError(e);
		}

		private void onMessage(object sender, MessageEventArgs e)
		{
			OnMessage(e);
		}

		private void onOpen(object sender, EventArgs e)
		{
			_id = _sessions.Add(this);
			if (_id == null)
			{
				_websocket.Close(CloseStatusCode.Away);
				return;
			}
			_startTime = DateTime.Now;
			OnOpen();
		}

		internal void Start(WebSocketContext context, WebSocketSessionManager sessions)
		{
			if (_websocket != null)
			{
				_websocket.Log.Error("This session has already been started.");
				context.WebSocket.Close(HttpStatusCode.ServiceUnavailable);
				return;
			}
			_context = context;
			_sessions = sessions;
			_websocket = context.WebSocket;
			_websocket.CustomHandshakeRequestChecker = checkIfValidConnectionRequest;
			_websocket.IgnoreExtensions = _ignoreExtensions;
			_websocket.Protocol = _protocol;
			TimeSpan waitTime = sessions.WaitTime;
			if (waitTime != _websocket.WaitTime)
			{
				_websocket.WaitTime = waitTime;
			}
			_websocket.OnOpen += onOpen;
			_websocket.OnMessage += onMessage;
			_websocket.OnError += onError;
			_websocket.OnClose += onClose;
			_websocket.ConnectAsServer();
		}

		protected void Error(string message, Exception exception)
		{
			if (message != null && message.Length > 0)
			{
				OnError(new ErrorEventArgs(message, exception));
			}
		}

		protected virtual void OnClose(CloseEventArgs e)
		{
		}

		protected virtual void OnError(ErrorEventArgs e)
		{
		}

		protected virtual void OnMessage(MessageEventArgs e)
		{
		}

		protected virtual void OnOpen()
		{
		}

		protected void Send(byte[] data)
		{
			if (_websocket != null)
			{
				_websocket.Send(data);
			}
		}

		protected void Send(FileInfo file)
		{
			if (_websocket != null)
			{
				_websocket.Send(file);
			}
		}

		protected void Send(string data)
		{
			if (_websocket != null)
			{
				_websocket.Send(data);
			}
		}

		protected void SendAsync(byte[] data, Action<bool> completed)
		{
			if (_websocket != null)
			{
				_websocket.SendAsync(data, completed);
			}
		}

		protected void SendAsync(FileInfo file, Action<bool> completed)
		{
			if (_websocket != null)
			{
				_websocket.SendAsync(file, completed);
			}
		}

		protected void SendAsync(string data, Action<bool> completed)
		{
			if (_websocket != null)
			{
				_websocket.SendAsync(data, completed);
			}
		}

		protected void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			if (_websocket != null)
			{
				_websocket.SendAsync(stream, length, completed);
			}
		}
	}
}
