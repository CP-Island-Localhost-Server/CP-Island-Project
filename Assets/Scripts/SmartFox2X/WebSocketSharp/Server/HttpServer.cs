using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public class HttpServer
	{
		private HttpListener _listener;

		private Logger _logger;

		private int _port;

		private Thread _receiveThread;

		private string _rootPath;

		private bool _secure;

		private WebSocketServiceManager _services;

		private volatile ServerState _state;

		private object _sync;

		private bool _windows;

		private EventHandler<HttpRequestEventArgs> m_OnConnect;

		private EventHandler<HttpRequestEventArgs> m_OnDelete;

		private EventHandler<HttpRequestEventArgs> m_OnGet;

		private EventHandler<HttpRequestEventArgs> m_OnHead;

		private EventHandler<HttpRequestEventArgs> m_OnOptions;

		private EventHandler<HttpRequestEventArgs> m_OnPatch;

		private EventHandler<HttpRequestEventArgs> m_OnPost;

		private EventHandler<HttpRequestEventArgs> m_OnPut;

		private EventHandler<HttpRequestEventArgs> m_OnTrace;

		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return _listener.AuthenticationSchemes;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_listener.AuthenticationSchemes = value;
				}
			}
		}

		public bool IsListening
		{
			get
			{
				return _state == ServerState.Start;
			}
		}

		public bool IsSecure
		{
			get
			{
				return _secure;
			}
		}

		public bool KeepClean
		{
			get
			{
				return _services.KeepClean;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_services.KeepClean = value;
				}
			}
		}

		public Logger Log
		{
			get
			{
				return _logger;
			}
		}

		public int Port
		{
			get
			{
				return _port;
			}
		}

		public string Realm
		{
			get
			{
				return _listener.Realm;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_listener.Realm = value;
				}
			}
		}

		public bool ReuseAddress
		{
			get
			{
				return _listener.ReuseAddress;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_listener.ReuseAddress = value;
				}
			}
		}

		public string RootPath
		{
			get
			{
				return (_rootPath == null || _rootPath.Length <= 0) ? (_rootPath = "./Public") : _rootPath;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_rootPath = value;
				}
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				return _listener.SslConfiguration;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_listener.SslConfiguration = value;
				}
			}
		}

		public Func<IIdentity, NetworkCredential> UserCredentialsFinder
		{
			get
			{
				return _listener.UserCredentialsFinder;
			}
			set
			{
				string text = _state.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_listener.UserCredentialsFinder = value;
				}
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return _services.WaitTime;
			}
			set
			{
				string text = _state.CheckIfStartable() ?? value.CheckIfValidWaitTime();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_services.WaitTime = value;
				}
			}
		}

		public WebSocketServiceManager WebSocketServices
		{
			get
			{
				return _services;
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnConnect
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnConnect;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnConnect, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnConnect;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnConnect, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnDelete
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnDelete;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnDelete, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnDelete;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnDelete, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnGet
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnGet;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnGet, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnGet;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnGet, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnHead
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnHead;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnHead, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnHead;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnHead, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnOptions
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnOptions;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnOptions, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnOptions;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnOptions, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnPatch
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnPatch;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnPatch, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnPatch;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnPatch, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnPost
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnPost;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnPost, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnPost;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnPost, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnPut
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnPut;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnPut, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnPut;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnPut, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<HttpRequestEventArgs> OnTrace
		{
			add
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnTrace;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnTrace, (EventHandler<HttpRequestEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<HttpRequestEventArgs> eventHandler = this.m_OnTrace;
				EventHandler<HttpRequestEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnTrace, (EventHandler<HttpRequestEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public HttpServer()
			: this(80, false)
		{
		}

		public HttpServer(int port)
			: this(port, port == 443)
		{
		}

		public HttpServer(int port, bool secure)
		{
			if (!port.IsPortNumber())
			{
				throw new ArgumentOutOfRangeException("port", "Not between 1 and 65535: " + port);
			}
			if ((port == 80 && secure) || (port == 443 && !secure))
			{
				throw new ArgumentException(string.Format("An invalid pair of 'port' and 'secure': {0}, {1}", port, secure));
			}
			_port = port;
			_secure = secure;
			_listener = new HttpListener();
			_logger = _listener.Log;
			_services = new WebSocketServiceManager(_logger);
			_state = ServerState.Ready;
			_sync = new object();
			OperatingSystem oSVersion = Environment.OSVersion;
			_windows = oSVersion.Platform != PlatformID.Unix && oSVersion.Platform != PlatformID.MacOSX;
			string uriPrefix = string.Format("http{0}://*:{1}/", (!_secure) ? "" : "s", _port);
			_listener.Prefixes.Add(uriPrefix);
		}

		private void abort()
		{
			lock (_sync)
			{
				if (!IsListening)
				{
					return;
				}
				_state = ServerState.ShuttingDown;
			}
			_services.Stop(new CloseEventArgs(CloseStatusCode.ServerError), true, false);
			_listener.Abort();
			_state = ServerState.Stop;
		}

		private string checkIfCertificateExists()
		{
			if (!_secure)
			{
				return null;
			}
			bool flag = _listener.SslConfiguration.ServerCertificate != null;
			bool flag2 = EndPointListener.CertificateExists(_port, _listener.CertificateFolderPath);
			if (flag && flag2)
			{
				_logger.Warn("The server certificate associated with the port number already exists.");
				return null;
			}
			return (flag || flag2) ? null : "The secure connection requires a server certificate.";
		}

		private void processRequest(HttpListenerContext context)
		{
			object obj;
			switch (context.Request.HttpMethod)
			{
			case "GET":
				obj = m_OnGet;
				break;
			case "HEAD":
				obj = m_OnHead;
				break;
			case "POST":
				obj = m_OnPost;
				break;
			case "PUT":
				obj = m_OnPut;
				break;
			case "DELETE":
				obj = m_OnDelete;
				break;
			case "OPTIONS":
				obj = m_OnOptions;
				break;
			case "TRACE":
				obj = m_OnTrace;
				break;
			case "CONNECT":
				obj = m_OnConnect;
				break;
			case "PATCH":
				obj = m_OnPatch;
				break;
			default:
				obj = null;
				break;
			}
			EventHandler<HttpRequestEventArgs> eventHandler = (EventHandler<HttpRequestEventArgs>)obj;
			if (eventHandler != null)
			{
				eventHandler(this, new HttpRequestEventArgs(context));
			}
			else
			{
				context.Response.StatusCode = 501;
			}
			context.Response.Close();
		}

		private void processRequest(HttpListenerWebSocketContext context)
		{
			WebSocketServiceHost host;
			if (!_services.InternalTryGetServiceHost(context.RequestUri.AbsolutePath, out host))
			{
				context.Close(HttpStatusCode.NotImplemented);
			}
			else
			{
				host.StartSession(context);
			}
		}

		private void receiveRequest()
		{
			while (true)
			{
				try
				{
					HttpListenerContext ctx = _listener.GetContext();
					ThreadPool.QueueUserWorkItem(delegate
					{
						try
						{
							if (ctx.Request.IsUpgradeTo("websocket"))
							{
								processRequest(ctx.AcceptWebSocket(null));
							}
							else
							{
								processRequest(ctx);
							}
						}
						catch (Exception ex3)
						{
							_logger.Fatal(ex3.ToString());
							ctx.Connection.Close(true);
						}
					});
				}
				catch (HttpListenerException ex)
				{
					_logger.Warn("Receiving has been stopped.\nreason: " + ex.Message);
					break;
				}
				catch (Exception ex2)
				{
					_logger.Fatal(ex2.ToString());
					break;
				}
			}
			if (IsListening)
			{
				abort();
			}
		}

		private void startReceiving()
		{
			_listener.Start();
			_receiveThread = new Thread(receiveRequest);
			_receiveThread.IsBackground = true;
			_receiveThread.Start();
		}

		private void stopReceiving(int millisecondsTimeout)
		{
			_listener.Close();
			_receiveThread.Join(millisecondsTimeout);
		}

		public void AddWebSocketService<TBehaviorWithNew>(string path) where TBehaviorWithNew : WebSocketBehavior, new()
		{
			AddWebSocketService(path, () => new TBehaviorWithNew());
		}

		public void AddWebSocketService<TBehavior>(string path, Func<TBehavior> initializer) where TBehavior : WebSocketBehavior
		{
			string text = path.CheckIfValidServicePath() ?? ((initializer != null) ? null : "'initializer' is null.");
			if (text != null)
			{
				_logger.Error(text);
			}
			else
			{
				_services.Add(path, initializer);
			}
		}

		public byte[] GetFile(string path)
		{
			path = RootPath + path;
			if (_windows)
			{
				path = path.Replace("/", "\\");
			}
			return (!File.Exists(path)) ? null : File.ReadAllBytes(path);
		}

		public bool RemoveWebSocketService(string path)
		{
			string text = path.CheckIfValidServicePath();
			if (text != null)
			{
				_logger.Error(text);
				return false;
			}
			return _services.Remove(path);
		}

		public void Start()
		{
			lock (_sync)
			{
				string text = _state.CheckIfStartable() ?? checkIfCertificateExists();
				if (text != null)
				{
					_logger.Error(text);
					return;
				}
				_services.Start();
				startReceiving();
				_state = ServerState.Start;
			}
		}

		public void Stop()
		{
			lock (_sync)
			{
				string text = _state.CheckIfStart();
				if (text != null)
				{
					_logger.Error(text);
					return;
				}
				_state = ServerState.ShuttingDown;
			}
			_services.Stop(new CloseEventArgs(), true, true);
			stopReceiving(5000);
			_state = ServerState.Stop;
		}

		public void Stop(ushort code, string reason)
		{
			lock (_sync)
			{
				string text = _state.CheckIfStart() ?? code.CheckIfValidCloseParameters(reason);
				if (text != null)
				{
					_logger.Error(text);
					return;
				}
				_state = ServerState.ShuttingDown;
			}
			if (code.IsNoStatusCode())
			{
				_services.Stop(new CloseEventArgs(), true, true);
			}
			else
			{
				bool flag = !code.IsReserved();
				_services.Stop(new CloseEventArgs(code, reason), flag, flag);
			}
			stopReceiving(5000);
			_state = ServerState.Stop;
		}

		public void Stop(CloseStatusCode code, string reason)
		{
			lock (_sync)
			{
				string text = _state.CheckIfStart() ?? code.CheckIfValidCloseParameters(reason);
				if (text != null)
				{
					_logger.Error(text);
					return;
				}
				_state = ServerState.ShuttingDown;
			}
			if (code.IsNoStatusCode())
			{
				_services.Stop(new CloseEventArgs(), true, true);
			}
			else
			{
				bool flag = !code.IsReserved();
				_services.Stop(new CloseEventArgs(code, reason), flag, flag);
			}
			stopReceiving(5000);
			_state = ServerState.Stop;
		}
	}
}
