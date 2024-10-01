using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp
{
	public class WebSocket : IDisposable
	{
		private const string _guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private const string _version = "13";

		internal const int FragmentLength = 2147483633;

		private AuthenticationChallenge _authChallenge;

		private string _base64Key;

		private bool _client;

		private Action _closeContext;

		private CompressionMethod _compression;

		private WebSocketContext _context;

		private CookieCollection _cookies;

		private NetworkCredential _credentials;

		private bool _enableRedirection;

		private string _extensions;

		private AutoResetEvent _exitReceiving;

		private object _forConn;

		private object _forEvent;

		private object _forMessageEventQueue;

		private object _forSend;

		private Func<WebSocketContext, string> _handshakeRequestChecker;

		private bool _ignoreExtensions;

		private volatile Logger _logger;

		private Queue<MessageEventArgs> _messageEventQueue;

		private uint _nonceCount;

		private string _origin;

		private bool _preAuth;

		private string _protocol;

		private string[] _protocols;

		private NetworkCredential _proxyCredentials;

		private Uri _proxyUri;

		private volatile WebSocketState _readyState;

		private AutoResetEvent _receivePong;

		private bool _secure;

		private ClientSslConfiguration _sslConfig;

		private Stream _stream;

		private TcpClient _tcpClient;

		private Uri _uri;

		private TimeSpan _waitTime;

		private EventHandler<CloseEventArgs> m_OnClose;

		private EventHandler<ErrorEventArgs> m_OnError;

		private EventHandler<MessageEventArgs> m_OnMessage;

		private EventHandler m_OnOpen;

		internal CookieCollection CookieCollection
		{
			get
			{
				return _cookies;
			}
		}

		internal Func<WebSocketContext, string> CustomHandshakeRequestChecker
		{
			get
			{
				return _handshakeRequestChecker ?? ((Func<WebSocketContext, string>)((WebSocketContext context) => (string)null));
			}
			set
			{
				_handshakeRequestChecker = value;
			}
		}

		internal bool IgnoreExtensions
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

		internal bool IsConnected
		{
			get
			{
				return _readyState == WebSocketState.Open || _readyState == WebSocketState.Closing;
			}
		}

		public CompressionMethod Compression
		{
			get
			{
				return _compression;
			}
			set
			{
				lock (_forConn)
				{
					string text = checkIfAvailable(false, false);
					if (text != null)
					{
						_logger.Error(text);
						error("An error has occurred in setting the compression.", null);
					}
					else
					{
						_compression = value;
					}
				}
			}
		}

		public IEnumerable<Cookie> Cookies
		{
			get
			{
				lock (_cookies.SyncRoot)
				{
					IEnumerator enumerator = _cookies.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							yield return (Cookie)enumerator.Current;
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
			}
		}

		public NetworkCredential Credentials
		{
			get
			{
				return _credentials;
			}
		}

		public bool EnableRedirection
		{
			get
			{
				return _enableRedirection;
			}
			set
			{
				lock (_forConn)
				{
					string text = checkIfAvailable(false, false);
					if (text != null)
					{
						_logger.Error(text);
						error("An error has occurred in setting the enable redirection.", null);
					}
					else
					{
						_enableRedirection = value;
					}
				}
			}
		}

		public string Extensions
		{
			get
			{
				return _extensions ?? string.Empty;
			}
		}

		public bool IsAlive
		{
			get
			{
				return Ping();
			}
		}

		public bool IsSecure
		{
			get
			{
				return _secure;
			}
		}

		public Logger Log
		{
			get
			{
				return _logger;
			}
			internal set
			{
				_logger = value;
			}
		}

		public string Origin
		{
			get
			{
				return _origin;
			}
			set
			{
				lock (_forConn)
				{
					string text = checkIfAvailable(false, false);
					if (text == null)
					{
						if (value.IsNullOrEmpty())
						{
							_origin = value;
							return;
						}
						Uri result;
						if (!Uri.TryCreate(value, UriKind.Absolute, out result) || result.Segments.Length > 1)
						{
							text = "The syntax of the origin must be '<scheme>://<host>[:<port>]'.";
						}
					}
					if (text != null)
					{
						_logger.Error(text);
						error("An error has occurred in setting the origin.", null);
					}
					else
					{
						_origin = value.TrimEnd('/');
					}
				}
			}
		}

		public string Protocol
		{
			get
			{
				return _protocol ?? string.Empty;
			}
			internal set
			{
				_protocol = value;
			}
		}

		public WebSocketState ReadyState
		{
			get
			{
				return _readyState;
			}
		}

		public ClientSslConfiguration SslConfiguration
		{
			get
			{
				return (!_client) ? null : (_sslConfig ?? (_sslConfig = new ClientSslConfiguration(_uri.DnsSafeHost)));
			}
			set
			{
				lock (_forConn)
				{
					string text = checkIfAvailable(false, false);
					if (text != null)
					{
						_logger.Error(text);
						error("An error has occurred in setting the ssl configuration.", null);
					}
					else
					{
						_sslConfig = value;
					}
				}
			}
		}

		public Uri Url
		{
			get
			{
				return (!_client) ? _context.RequestUri : _uri;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return _waitTime;
			}
			set
			{
				lock (_forConn)
				{
					string text = checkIfAvailable(true, false) ?? value.CheckIfValidWaitTime();
					if (text != null)
					{
						_logger.Error(text);
						error("An error has occurred in setting the wait time.", null);
					}
					else
					{
						_waitTime = value;
					}
				}
			}
		}

		public event EventHandler<CloseEventArgs> OnClose
		{
			add
			{
				EventHandler<CloseEventArgs> eventHandler = this.m_OnClose;
				EventHandler<CloseEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnClose, (EventHandler<CloseEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CloseEventArgs> eventHandler = this.m_OnClose;
				EventHandler<CloseEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnClose, (EventHandler<CloseEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<ErrorEventArgs> OnError
		{
			add
			{
				EventHandler<ErrorEventArgs> eventHandler = this.m_OnError;
				EventHandler<ErrorEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnError, (EventHandler<ErrorEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<ErrorEventArgs> eventHandler = this.m_OnError;
				EventHandler<ErrorEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnError, (EventHandler<ErrorEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<MessageEventArgs> OnMessage
		{
			add
			{
				EventHandler<MessageEventArgs> eventHandler = this.m_OnMessage;
				EventHandler<MessageEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnMessage, (EventHandler<MessageEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<MessageEventArgs> eventHandler = this.m_OnMessage;
				EventHandler<MessageEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnMessage, (EventHandler<MessageEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler OnOpen
		{
			add
			{
				EventHandler eventHandler = this.m_OnOpen;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnOpen, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = this.m_OnOpen;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref this.m_OnOpen, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		internal WebSocket(HttpListenerWebSocketContext context, string protocol)
		{
			_context = context;
			_protocol = protocol;
			_closeContext = context.Close;
			_logger = context.Log;
			_secure = context.IsSecureConnection;
			_stream = context.Stream;
			_waitTime = TimeSpan.FromSeconds(1.0);
			init();
		}

		internal WebSocket(TcpListenerWebSocketContext context, string protocol)
		{
			_context = context;
			_protocol = protocol;
			_closeContext = context.Close;
			_logger = context.Log;
			_secure = context.IsSecureConnection;
			_stream = context.Stream;
			_waitTime = TimeSpan.FromSeconds(1.0);
			init();
		}

		public WebSocket(string url, params string[] protocols)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			string message;
			if (!url.TryCreateWebSocketUri(out _uri, out message))
			{
				throw new ArgumentException(message, "url");
			}
			if (protocols != null && protocols.Length > 0)
			{
				message = protocols.CheckIfValidProtocols();
				if (message != null)
				{
					throw new ArgumentException(message, "protocols");
				}
				_protocols = protocols;
			}
			_base64Key = CreateBase64Key();
			_client = true;
			_logger = new Logger();
			_secure = _uri.Scheme == "wss";
			_waitTime = TimeSpan.FromSeconds(5.0);
			init();
		}

		void IDisposable.Dispose()
		{
			close(new CloseEventArgs(CloseStatusCode.Away), true, true);
		}

		private bool acceptHandshake()
		{
			_logger.Debug(string.Format("A connection request from {0}:\n{1}", _context.UserEndPoint, _context));
			string text = checkIfValidHandshakeRequest(_context);
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred while connecting.", null);
				Close(HttpStatusCode.BadRequest);
				return false;
			}
			if (_protocol != null && !_context.SecWebSocketProtocols.Contains((string protocol) => protocol == _protocol))
			{
				_protocol = null;
			}
			if (!_ignoreExtensions)
			{
				string text2 = _context.Headers["Sec-WebSocket-Extensions"];
				if (text2 != null && text2.Length > 0)
				{
					processSecWebSocketExtensionsHeader(text2);
				}
			}
			return sendHttpResponse(createHandshakeResponse());
		}

		private string checkIfAvailable(bool asServer, bool asConnected)
		{
			return (!_client && !asServer) ? "This operation isn't available as a server." : (asConnected ? null : _readyState.CheckIfConnectable());
		}

		private string checkIfCanConnect()
		{
			return (_client || _readyState != WebSocketState.Closed) ? _readyState.CheckIfConnectable() : "Connect isn't available to reconnect as a server.";
		}

		private string checkIfValidHandshakeRequest(WebSocketContext context)
		{
			NameValueCollection headers = context.Headers;
			return (context.RequestUri == null) ? "Specifies an invalid Request-URI." : ((!context.IsWebSocketRequest) ? "Not a WebSocket connection request." : ((!validateSecWebSocketKeyHeader(headers["Sec-WebSocket-Key"])) ? "Includes an invalid Sec-WebSocket-Key header." : (validateSecWebSocketVersionClientHeader(headers["Sec-WebSocket-Version"]) ? CustomHandshakeRequestChecker(context) : "Includes an invalid Sec-WebSocket-Version header.")));
		}

		private string checkIfValidHandshakeResponse(HttpResponse response)
		{
			NameValueCollection headers = response.Headers;
			return response.IsRedirect ? "Indicates the redirection." : (response.IsUnauthorized ? "Requires the authentication." : ((!response.IsWebSocketResponse) ? "Not a WebSocket connection response." : ((!validateSecWebSocketAcceptHeader(headers["Sec-WebSocket-Accept"])) ? "Includes an invalid Sec-WebSocket-Accept header." : ((!validateSecWebSocketProtocolHeader(headers["Sec-WebSocket-Protocol"])) ? "Includes an invalid Sec-WebSocket-Protocol header." : ((!validateSecWebSocketExtensionsHeader(headers["Sec-WebSocket-Extensions"])) ? "Includes an invalid Sec-WebSocket-Extensions header." : (validateSecWebSocketVersionServerHeader(headers["Sec-WebSocket-Version"]) ? null : "Includes an invalid Sec-WebSocket-Version header."))))));
		}

		private string checkIfValidReceivedFrame(WebSocketFrame frame)
		{
			bool isMasked = frame.IsMasked;
			return (_client && isMasked) ? "A frame from the server is masked." : ((!_client && !isMasked) ? "A frame from a client isn't masked." : ((!frame.IsCompressed || _compression != 0) ? null : "A compressed frame is without the available decompression method."));
		}

		private void close(CloseEventArgs e, bool send, bool wait)
		{
			lock (_forConn)
			{
				if (_readyState == WebSocketState.Closing || _readyState == WebSocketState.Closed)
				{
					_logger.Info("Closing the connection has already been done.");
					return;
				}
				send = send && _readyState == WebSocketState.Open;
				wait = wait && send;
				_readyState = WebSocketState.Closing;
			}
			_logger.Trace("Start closing the connection.");
			e.WasClean = closeHandshake((!send) ? null : WebSocketFrame.CreateCloseFrame(e.PayloadData, _client).ToByteArray(), (!wait) ? TimeSpan.Zero : _waitTime, (!_client) ? new Action(releaseServerResources) : new Action(releaseClientResources));
			_logger.Trace("End closing the connection.");
			_readyState = WebSocketState.Closed;
			try
			{
				m_OnClose.Emit(this, e);
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
				error("An exception has occurred during an OnClose event.", ex);
			}
		}

		private void closeAsync(CloseEventArgs e, bool send, bool wait)
		{
			Action<CloseEventArgs, bool, bool> closer = close;
			closer.BeginInvoke(e, send, wait, delegate(IAsyncResult ar)
			{
				closer.EndInvoke(ar);
			}, null);
		}

		private bool closeHandshake(byte[] frameAsBytes, TimeSpan timeout, Action release)
		{
			bool flag = frameAsBytes != null && sendBytes(frameAsBytes);
			bool flag2 = timeout == TimeSpan.Zero || (flag && _exitReceiving != null && _exitReceiving.WaitOne(timeout));
			release();
			if (_receivePong != null)
			{
				_receivePong.Close();
				_receivePong = null;
			}
			if (_exitReceiving != null)
			{
				_exitReceiving.Close();
				_exitReceiving = null;
			}
			bool flag3 = flag && flag2;
			_logger.Debug(string.Format("Was clean?: {0}\nsent: {1} received: {2}", flag3, flag, flag2));
			return flag3;
		}

		private bool concatenateFragmentsInto(Stream destination)
		{
			WebSocketFrame webSocketFrame;
			while (true)
			{
				webSocketFrame = WebSocketFrame.Read(_stream, false);
				string text = checkIfValidReceivedFrame(webSocketFrame);
				if (text != null)
				{
					return processUnsupportedFrame(webSocketFrame, CloseStatusCode.ProtocolError, text);
				}
				webSocketFrame.Unmask();
				if (webSocketFrame.IsFinal)
				{
					if (webSocketFrame.IsContinuation)
					{
						destination.WriteBytes(webSocketFrame.PayloadData.ApplicationData);
						return true;
					}
					if (webSocketFrame.IsPing)
					{
						processPingFrame(webSocketFrame);
						continue;
					}
					if (!webSocketFrame.IsPong)
					{
						if (webSocketFrame.IsClose)
						{
							return processCloseFrame(webSocketFrame);
						}
						break;
					}
					processPongFrame(webSocketFrame);
				}
				else
				{
					if (!webSocketFrame.IsContinuation)
					{
						break;
					}
					destination.WriteBytes(webSocketFrame.PayloadData.ApplicationData);
				}
			}
			return processUnsupportedFrame(webSocketFrame, CloseStatusCode.IncorrectData, "An incorrect data has been received while receiving the fragmented data.");
		}

		private bool connect()
		{
			lock (_forConn)
			{
				string text = _readyState.CheckIfConnectable();
				if (text != null)
				{
					_logger.Error(text);
					error("An error has occurred in connecting.", null);
					return false;
				}
				try
				{
					_readyState = WebSocketState.Connecting;
					if ((!_client) ? acceptHandshake() : doHandshake())
					{
						_readyState = WebSocketState.Open;
						return true;
					}
				}
				catch (Exception exception)
				{
					processException(exception, "An exception has occurred while connecting.");
				}
				return false;
			}
		}

		private string createExtensions()
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			if (_compression != 0)
			{
				string arg = _compression.ToExtensionString("server_no_context_takeover", "client_no_context_takeover");
				stringBuilder.AppendFormat("{0}, ", arg);
			}
			int length = stringBuilder.Length;
			if (length > 2)
			{
				stringBuilder.Length = length - 2;
				return stringBuilder.ToString();
			}
			return null;
		}

		private HttpResponse createHandshakeCloseResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = HttpResponse.CreateCloseResponse(code);
			httpResponse.Headers["Sec-WebSocket-Version"] = "13";
			return httpResponse;
		}

		private HttpRequest createHandshakeRequest()
		{
			HttpRequest httpRequest = HttpRequest.CreateWebSocketRequest(_uri);
			NameValueCollection headers = httpRequest.Headers;
			if (!_origin.IsNullOrEmpty())
			{
				headers["Origin"] = _origin;
			}
			headers["Sec-WebSocket-Key"] = _base64Key;
			if (_protocols != null)
			{
				headers["Sec-WebSocket-Protocol"] = _protocols.ToString(", ");
			}
			string text = createExtensions();
			if (text != null)
			{
				headers["Sec-WebSocket-Extensions"] = text;
			}
			headers["Sec-WebSocket-Version"] = "13";
			AuthenticationResponse authenticationResponse = null;
			if (_authChallenge != null && _credentials != null)
			{
				authenticationResponse = new AuthenticationResponse(_authChallenge, _credentials, _nonceCount);
				_nonceCount = authenticationResponse.NonceCount;
			}
			else if (_preAuth)
			{
				authenticationResponse = new AuthenticationResponse(_credentials);
			}
			if (authenticationResponse != null)
			{
				headers["Authorization"] = authenticationResponse.ToString();
			}
			if (_cookies.Count > 0)
			{
				httpRequest.SetCookies(_cookies);
			}
			return httpRequest;
		}

		private HttpResponse createHandshakeResponse()
		{
			HttpResponse httpResponse = HttpResponse.CreateWebSocketResponse();
			NameValueCollection headers = httpResponse.Headers;
			headers["Sec-WebSocket-Accept"] = CreateResponseKey(_base64Key);
			if (_protocol != null)
			{
				headers["Sec-WebSocket-Protocol"] = _protocol;
			}
			if (_extensions != null)
			{
				headers["Sec-WebSocket-Extensions"] = _extensions;
			}
			if (_cookies.Count > 0)
			{
				httpResponse.SetCookies(_cookies);
			}
			return httpResponse;
		}

		private MessageEventArgs dequeueFromMessageEventQueue()
		{
			lock (_forMessageEventQueue)
			{
				return (_messageEventQueue.Count <= 0) ? null : _messageEventQueue.Dequeue();
			}
		}

		private bool doHandshake()
		{
			setClientStream();
			HttpResponse httpResponse = sendHandshakeRequest();
			string text = checkIfValidHandshakeResponse(httpResponse);
			if (text != null)
			{
				_logger.Error(text);
				text = "An error has occurred while connecting.";
				error(text, null);
				close(new CloseEventArgs(CloseStatusCode.Abnormal, text), false, false);
				return false;
			}
			CookieCollection cookies = httpResponse.Cookies;
			if (cookies.Count > 0)
			{
				_cookies.SetOrRemove(cookies);
			}
			return true;
		}

		private void enqueueToMessageEventQueue(MessageEventArgs e)
		{
			lock (_forMessageEventQueue)
			{
				_messageEventQueue.Enqueue(e);
			}
		}

		private void error(string message, Exception exception)
		{
			try
			{
				m_OnError.Emit(this, new ErrorEventArgs(message, exception));
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
			}
		}

		private void init()
		{
			_compression = CompressionMethod.None;
			_cookies = new CookieCollection();
			_forConn = new object();
			_forEvent = new object();
			_forSend = new object();
			_messageEventQueue = new Queue<MessageEventArgs>();
			_forMessageEventQueue = ((ICollection)_messageEventQueue).SyncRoot;
			_readyState = WebSocketState.Connecting;
		}

		private void open()
		{
			try
			{
				startReceiving();
				lock (_forEvent)
				{
					try
					{
						m_OnOpen.Emit(this, EventArgs.Empty);
					}
					catch (Exception exception)
					{
						processException(exception, "An exception has occurred during an OnOpen event.");
					}
				}
			}
			catch (Exception exception2)
			{
				processException(exception2, "An exception has occurred while opening.");
			}
		}

		private bool processCloseFrame(WebSocketFrame frame)
		{
			PayloadData payloadData = frame.PayloadData;
			close(new CloseEventArgs(payloadData), !payloadData.IncludesReservedCloseStatusCode, false);
			return false;
		}

		private bool processDataFrame(WebSocketFrame frame)
		{
			enqueueToMessageEventQueue((!frame.IsCompressed) ? new MessageEventArgs(frame) : new MessageEventArgs(frame.Opcode, frame.PayloadData.ApplicationData.Decompress(_compression)));
			return true;
		}

		private void processException(Exception exception, string message)
		{
			CloseStatusCode closeStatusCode = CloseStatusCode.Abnormal;
			string text = message;
			if (exception is WebSocketException)
			{
				WebSocketException ex = (WebSocketException)exception;
				closeStatusCode = ex.Code;
				text = ex.Message;
			}
			if (closeStatusCode == CloseStatusCode.Abnormal || closeStatusCode == CloseStatusCode.TlsHandshakeFailure)
			{
				_logger.Fatal(exception.ToString());
			}
			else
			{
				_logger.Error(text);
			}
			error(message ?? closeStatusCode.GetMessage(), exception);
			if (!_client && _readyState == WebSocketState.Connecting)
			{
				Close(HttpStatusCode.BadRequest);
			}
			else
			{
				close(new CloseEventArgs(closeStatusCode, text ?? closeStatusCode.GetMessage()), !closeStatusCode.IsReserved(), false);
			}
		}

		private bool processFragmentedFrame(WebSocketFrame frame)
		{
			return frame.IsContinuation || processFragments(frame);
		}

		private bool processFragments(WebSocketFrame first)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.WriteBytes(first.PayloadData.ApplicationData);
				if (!concatenateFragmentsInto(memoryStream))
				{
					return false;
				}
				byte[] rawData;
				if (_compression != 0)
				{
					rawData = memoryStream.DecompressToArray(_compression);
				}
				else
				{
					memoryStream.Close();
					rawData = memoryStream.ToArray();
				}
				enqueueToMessageEventQueue(new MessageEventArgs(first.Opcode, rawData));
				return true;
			}
		}

		private bool processPingFrame(WebSocketFrame frame)
		{
			if (send(new WebSocketFrame(Opcode.Pong, frame.PayloadData, _client).ToByteArray()))
			{
				_logger.Trace("Returned a Pong.");
			}
			return true;
		}

		private bool processPongFrame(WebSocketFrame frame)
		{
			_receivePong.Set();
			_logger.Trace("Received a Pong.");
			return true;
		}

		private bool processReceivedFrame(WebSocketFrame frame)
		{
			string text = checkIfValidReceivedFrame(frame);
			if (text != null)
			{
				return processUnsupportedFrame(frame, CloseStatusCode.ProtocolError, text);
			}
			frame.Unmask();
			return frame.IsFragmented ? processFragmentedFrame(frame) : (frame.IsData ? processDataFrame(frame) : (frame.IsPing ? processPingFrame(frame) : (frame.IsPong ? processPongFrame(frame) : ((!frame.IsClose) ? processUnsupportedFrame(frame, CloseStatusCode.IncorrectData, null) : processCloseFrame(frame)))));
		}

		private void processSecWebSocketExtensionsHeader(string value)
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			bool flag = false;
			foreach (string item in value.SplitHeaderValue(','))
			{
				string value2 = item.Trim();
				if (!flag && value2.IsCompressionExtension(CompressionMethod.Deflate))
				{
					_compression = CompressionMethod.Deflate;
					string arg = _compression.ToExtensionString("client_no_context_takeover", "server_no_context_takeover");
					stringBuilder.AppendFormat("{0}, ", arg);
					flag = true;
				}
			}
			int length = stringBuilder.Length;
			if (length > 2)
			{
				stringBuilder.Length = length - 2;
				_extensions = stringBuilder.ToString();
			}
		}

		private bool processUnsupportedFrame(WebSocketFrame frame, CloseStatusCode code, string reason)
		{
			_logger.Debug("An unsupported frame:" + frame.PrintToString(false));
			processException(new WebSocketException(code, reason), null);
			return false;
		}

		private void releaseClientResources()
		{
			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
			if (_tcpClient != null)
			{
				_tcpClient.Close();
				_tcpClient = null;
			}
		}

		private void releaseServerResources()
		{
			if (_closeContext != null)
			{
				_closeContext();
				_closeContext = null;
				_stream = null;
				_context = null;
			}
		}

		private bool send(byte[] frameAsBytes)
		{
			lock (_forConn)
			{
				if (_readyState != WebSocketState.Open)
				{
					_logger.Error("Closing the connection has been done.");
					return false;
				}
				return sendBytes(frameAsBytes);
			}
		}

		private bool send(Opcode opcode, Stream stream)
		{
			lock (_forSend)
			{
				Stream stream2 = stream;
				bool flag = false;
				bool flag2 = false;
				try
				{
					if (_compression != 0)
					{
						stream = stream.Compress(_compression);
						flag = true;
					}
					flag2 = send(opcode, stream, flag);
					if (!flag2)
					{
						error("Sending the data has been interrupted.", null);
					}
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred while sending the data.", ex);
				}
				finally
				{
					if (flag)
					{
						stream.Dispose();
					}
					stream2.Dispose();
				}
				return flag2;
			}
		}

		private bool send(Opcode opcode, Stream stream, bool compressed)
		{
			long length = stream.Length;
			if (length == 0)
			{
				return send(Fin.Final, opcode, new byte[0], compressed);
			}
			long num = length / 2147483633;
			int num2 = (int)(length % 2147483633);
			byte[] array = null;
			if (num == 0)
			{
				array = new byte[num2];
				return stream.Read(array, 0, num2) == num2 && send(Fin.Final, opcode, array, compressed);
			}
			array = new byte[2147483633];
			if (num == 1 && num2 == 0)
			{
				return stream.Read(array, 0, 2147483633) == 2147483633 && send(Fin.Final, opcode, array, compressed);
			}
			if (stream.Read(array, 0, 2147483633) != 2147483633 || !send(Fin.More, opcode, array, compressed))
			{
				return false;
			}
			long num3 = ((num2 != 0) ? (num - 1) : (num - 2));
			for (long num4 = 0L; num4 < num3; num4++)
			{
				if (stream.Read(array, 0, 2147483633) != 2147483633 || !send(Fin.More, Opcode.Cont, array, compressed))
				{
					return false;
				}
			}
			if (num2 == 0)
			{
				num2 = 2147483633;
			}
			else
			{
				array = new byte[num2];
			}
			return stream.Read(array, 0, num2) == num2 && send(Fin.Final, Opcode.Cont, array, compressed);
		}

		private bool send(Fin fin, Opcode opcode, byte[] data, bool compressed)
		{
			lock (_forConn)
			{
				if (_readyState != WebSocketState.Open)
				{
					_logger.Error("Closing the connection has been done.");
					return false;
				}
				return sendBytes(new WebSocketFrame(fin, opcode, data, compressed, _client).ToByteArray());
			}
		}

		private void sendAsync(Opcode opcode, Stream stream, Action<bool> completed)
		{
			Func<Opcode, Stream, bool> sender = send;
			sender.BeginInvoke(opcode, stream, delegate(IAsyncResult ar)
			{
				try
				{
					bool obj = sender.EndInvoke(ar);
					if (completed != null)
					{
						completed(obj);
					}
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
					error("An exception has occurred during a send callback.", ex);
				}
			}, null);
		}

		private bool sendBytes(byte[] bytes)
		{
			try
			{
				_stream.Write(bytes, 0, bytes.Length);
				return true;
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
				return false;
			}
		}

		private HttpResponse sendHandshakeRequest()
		{
			HttpRequest httpRequest = createHandshakeRequest();
			HttpResponse httpResponse = sendHttpRequest(httpRequest, 90000);
			if (httpResponse.IsUnauthorized)
			{
				string text = httpResponse.Headers["WWW-Authenticate"];
				_logger.Warn(string.Format("Received an authentication requirement for '{0}'.", text));
				if (text.IsNullOrEmpty())
				{
					_logger.Error("No authentication challenge is specified.");
					return httpResponse;
				}
				_authChallenge = AuthenticationChallenge.Parse(text);
				if (_authChallenge == null)
				{
					_logger.Error("An invalid authentication challenge is specified.");
					return httpResponse;
				}
				if (_credentials != null && (!_preAuth || _authChallenge.Scheme == AuthenticationSchemes.Digest))
				{
					if (httpResponse.HasConnectionClose)
					{
						releaseClientResources();
						setClientStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(_authChallenge, _credentials, _nonceCount);
					_nonceCount = authenticationResponse.NonceCount;
					httpRequest.Headers["Authorization"] = authenticationResponse.ToString();
					httpResponse = sendHttpRequest(httpRequest, 15000);
				}
			}
			if (httpResponse.IsRedirect)
			{
				string text2 = httpResponse.Headers["Location"];
				_logger.Warn(string.Format("Received a redirection to '{0}'.", text2));
				if (_enableRedirection)
				{
					if (text2.IsNullOrEmpty())
					{
						_logger.Error("No url to redirect is located.");
						return httpResponse;
					}
					Uri result;
					string message;
					if (!text2.TryCreateWebSocketUri(out result, out message))
					{
						_logger.Error("An invalid url to redirect is located: " + message);
						return httpResponse;
					}
					releaseClientResources();
					_uri = result;
					_secure = result.Scheme == "wss";
					setClientStream();
					return sendHandshakeRequest();
				}
			}
			return httpResponse;
		}

		private HttpResponse sendHttpRequest(HttpRequest request, int millisecondsTimeout)
		{
			_logger.Debug("A request to the server:\n" + request.ToString());
			HttpResponse response = request.GetResponse(_stream, millisecondsTimeout);
			_logger.Debug("A response to this request:\n" + response.ToString());
			return response;
		}

		private bool sendHttpResponse(HttpResponse response)
		{
			_logger.Debug("A response to this request:\n" + response.ToString());
			return sendBytes(response.ToByteArray());
		}

		private void sendProxyConnectRequest()
		{
			HttpRequest httpRequest = HttpRequest.CreateConnectRequest(_uri);
			HttpResponse httpResponse = sendHttpRequest(httpRequest, 90000);
			if (httpResponse.IsProxyAuthenticationRequired)
			{
				string text = httpResponse.Headers["Proxy-Authenticate"];
				_logger.Warn(string.Format("Received a proxy authentication requirement for '{0}'.", text));
				if (text.IsNullOrEmpty())
				{
					throw new WebSocketException("No proxy authentication challenge is specified.");
				}
				AuthenticationChallenge authenticationChallenge = AuthenticationChallenge.Parse(text);
				if (authenticationChallenge == null)
				{
					throw new WebSocketException("An invalid proxy authentication challenge is specified.");
				}
				if (_proxyCredentials != null)
				{
					if (httpResponse.HasConnectionClose)
					{
						releaseClientResources();
						_tcpClient = new TcpClient(_proxyUri.DnsSafeHost, _proxyUri.Port);
						_stream = _tcpClient.GetStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(authenticationChallenge, _proxyCredentials, 0u);
					httpRequest.Headers["Proxy-Authorization"] = authenticationResponse.ToString();
					httpResponse = sendHttpRequest(httpRequest, 15000);
				}
				if (httpResponse.IsProxyAuthenticationRequired)
				{
					throw new WebSocketException("A proxy authentication is required.");
				}
			}
			if (httpResponse.StatusCode[0] != '2')
			{
				throw new WebSocketException("The proxy has failed a connection to the requested host and port.");
			}
		}

		private void setClientStream()
		{
			if (_proxyUri != null)
			{
				_tcpClient = new TcpClient(_proxyUri.DnsSafeHost, _proxyUri.Port);
				_stream = _tcpClient.GetStream();
				sendProxyConnectRequest();
			}
			else
			{
				_tcpClient = new TcpClient(_uri.DnsSafeHost, _uri.Port);
				_stream = _tcpClient.GetStream();
			}
			if (_secure)
			{
				ClientSslConfiguration sslConfiguration = SslConfiguration;
				string targetHost = sslConfiguration.TargetHost;
				if (targetHost != _uri.DnsSafeHost)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, "An invalid host name is specified.");
				}
				try
				{
					SslStream sslStream = new SslStream(_stream, false, sslConfiguration.ServerCertificateValidationCallback, sslConfiguration.ClientCertificateSelectionCallback);
					sslStream.AuthenticateAsClient(targetHost, sslConfiguration.ClientCertificates, sslConfiguration.EnabledSslProtocols, sslConfiguration.CheckCertificateRevocation);
					_stream = sslStream;
				}
				catch (Exception innerException)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, innerException);
				}
			}
		}

		private void startReceiving()
		{
			if (_messageEventQueue.Count > 0)
			{
				_messageEventQueue.Clear();
			}
			_exitReceiving = new AutoResetEvent(false);
			_receivePong = new AutoResetEvent(false);
			Action receive = null;
			receive = delegate
			{
				WebSocketFrame.ReadAsync(_stream, false, delegate(WebSocketFrame frame)
				{
					if (processReceivedFrame(frame) && _readyState != WebSocketState.Closed)
					{
						receive();
						if (frame.IsData)
						{
							lock (_forEvent)
							{
								try
								{
									MessageEventArgs messageEventArgs = dequeueFromMessageEventQueue();
									if (messageEventArgs != null && _readyState == WebSocketState.Open)
									{
										m_OnMessage.Emit(this, messageEventArgs);
									}
								}
								catch (Exception exception)
								{
									processException(exception, "An exception has occurred during an OnMessage event.");
								}
							}
						}
					}
					else if (_exitReceiving != null)
					{
						_exitReceiving.Set();
					}
				}, delegate(Exception ex)
				{
					processException(ex, "An exception has occurred while receiving a message.");
				});
			};
			receive();
		}

		private bool validateSecWebSocketAcceptHeader(string value)
		{
			return value != null && value == CreateResponseKey(_base64Key);
		}

		private bool validateSecWebSocketExtensionsHeader(string value)
		{
			bool flag = _compression != CompressionMethod.None;
			if (value == null || value.Length == 0)
			{
				if (flag)
				{
					_compression = CompressionMethod.None;
				}
				return true;
			}
			if (!flag)
			{
				return false;
			}
			foreach (string item in value.SplitHeaderValue(','))
			{
				string text = item.Trim();
				if (text.IsCompressionExtension(_compression))
				{
					if (!text.Contains("server_no_context_takeover"))
					{
						_logger.Error("The server hasn't sent back 'server_no_context_takeover'.");
						return false;
					}
					if (!text.Contains("client_no_context_takeover"))
					{
						_logger.Warn("The server hasn't sent back 'client_no_context_takeover'.");
					}
					string method = _compression.ToExtensionString();
					if (text.SplitHeaderValue(';').Contains(delegate(string t)
					{
						t = t.Trim();
						return t != method && t != "server_no_context_takeover" && t != "client_no_context_takeover";
					}))
					{
						return false;
					}
					continue;
				}
				return false;
			}
			_extensions = value;
			return true;
		}

		private bool validateSecWebSocketKeyHeader(string value)
		{
			if (value == null || value.Length == 0)
			{
				return false;
			}
			_base64Key = value;
			return true;
		}

		private bool validateSecWebSocketProtocolHeader(string value)
		{
			if (value == null)
			{
				return _protocols == null;
			}
			if (_protocols == null || !_protocols.Contains((string protocol) => protocol == value))
			{
				return false;
			}
			_protocol = value;
			return true;
		}

		private bool validateSecWebSocketVersionClientHeader(string value)
		{
			return value != null && value == "13";
		}

		private bool validateSecWebSocketVersionServerHeader(string value)
		{
			return value == null || value == "13";
		}

		internal void Close(HttpResponse response)
		{
			_readyState = WebSocketState.Closing;
			sendHttpResponse(response);
			releaseServerResources();
			_readyState = WebSocketState.Closed;
		}

		internal void Close(HttpStatusCode code)
		{
			Close(createHandshakeCloseResponse(code));
		}

		internal void Close(CloseEventArgs e, byte[] frameAsBytes, TimeSpan timeout)
		{
			lock (_forConn)
			{
				if (_readyState == WebSocketState.Closing || _readyState == WebSocketState.Closed)
				{
					_logger.Info("Closing the connection has already been done.");
					return;
				}
				_readyState = WebSocketState.Closing;
			}
			e.WasClean = closeHandshake(frameAsBytes, timeout, releaseServerResources);
			_readyState = WebSocketState.Closed;
			try
			{
				m_OnClose.Emit(this, e);
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
			}
		}

		internal void ConnectAsServer()
		{
			try
			{
				if (acceptHandshake())
				{
					_readyState = WebSocketState.Open;
					open();
				}
			}
			catch (Exception exception)
			{
				processException(exception, "An exception has occurred while connecting.");
			}
		}

		internal static string CreateBase64Key()
		{
			byte[] array = new byte[16];
			Random random = new Random();
			random.NextBytes(array);
			return Convert.ToBase64String(array);
		}

		internal static string CreateResponseKey(string base64Key)
		{
			StringBuilder stringBuilder = new StringBuilder(base64Key, 64);
			stringBuilder.Append("258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
			SHA1 sHA = new SHA1CryptoServiceProvider();
			byte[] inArray = sHA.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			return Convert.ToBase64String(inArray);
		}

		internal bool Ping(byte[] frameAsBytes, TimeSpan timeout)
		{
			try
			{
				AutoResetEvent receivePong;
				return _readyState == WebSocketState.Open && send(frameAsBytes) && (receivePong = _receivePong) != null && receivePong.WaitOne(timeout);
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
				return false;
			}
		}

		internal void Send(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			lock (_forSend)
			{
				lock (_forConn)
				{
					if (_readyState != WebSocketState.Open)
					{
						_logger.Error("Closing the connection has been done.");
						return;
					}
					try
					{
						byte[] value;
						if (!cache.TryGetValue(_compression, out value))
						{
							value = new WebSocketFrame(Fin.Final, opcode, data.Compress(_compression), _compression != CompressionMethod.None, false).ToByteArray();
							cache.Add(_compression, value);
						}
						sendBytes(value);
					}
					catch (Exception ex)
					{
						_logger.Fatal(ex.ToString());
					}
				}
			}
		}

		internal void Send(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			lock (_forSend)
			{
				try
				{
					Stream value;
					if (!cache.TryGetValue(_compression, out value))
					{
						value = stream.Compress(_compression);
						cache.Add(_compression, value);
					}
					else
					{
						value.Position = 0L;
					}
					send(opcode, value, _compression != CompressionMethod.None);
				}
				catch (Exception ex)
				{
					_logger.Fatal(ex.ToString());
				}
			}
		}

		public void Close()
		{
			string text = _readyState.CheckIfClosable();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else
			{
				close(new CloseEventArgs(), true, true);
			}
		}

		public void Close(ushort code)
		{
			string text = _readyState.CheckIfClosable() ?? code.CheckIfValidCloseStatusCode();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				close(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				close(new CloseEventArgs(code), wait, wait);
			}
		}

		public void Close(CloseStatusCode code)
		{
			string text = _readyState.CheckIfClosable();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				close(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				close(new CloseEventArgs(code), wait, wait);
			}
		}

		public void Close(ushort code, string reason)
		{
			string text = _readyState.CheckIfClosable() ?? code.CheckIfValidCloseParameters(reason);
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				close(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				close(new CloseEventArgs(code, reason), wait, wait);
			}
		}

		public void Close(CloseStatusCode code, string reason)
		{
			string text = _readyState.CheckIfClosable() ?? code.CheckIfValidCloseParameters(reason);
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				close(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				close(new CloseEventArgs(code, reason), wait, wait);
			}
		}

		public void CloseAsync()
		{
			string text = _readyState.CheckIfClosable();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else
			{
				closeAsync(new CloseEventArgs(), true, true);
			}
		}

		public void CloseAsync(ushort code)
		{
			string text = _readyState.CheckIfClosable() ?? code.CheckIfValidCloseStatusCode();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				closeAsync(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				closeAsync(new CloseEventArgs(code), wait, wait);
			}
		}

		public void CloseAsync(CloseStatusCode code)
		{
			string text = _readyState.CheckIfClosable();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				closeAsync(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				closeAsync(new CloseEventArgs(code), wait, wait);
			}
		}

		public void CloseAsync(ushort code, string reason)
		{
			string text = _readyState.CheckIfClosable() ?? code.CheckIfValidCloseParameters(reason);
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				closeAsync(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				closeAsync(new CloseEventArgs(code, reason), wait, wait);
			}
		}

		public void CloseAsync(CloseStatusCode code, string reason)
		{
			string text = _readyState.CheckIfClosable() ?? code.CheckIfValidCloseParameters(reason);
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in closing the connection.", null);
			}
			else if (code.IsNoStatusCode())
			{
				closeAsync(new CloseEventArgs(), true, true);
			}
			else
			{
				bool wait = !code.IsReserved();
				closeAsync(new CloseEventArgs(code, reason), wait, wait);
			}
		}

		public void Connect()
		{
			string text = checkIfCanConnect();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in connecting.", null);
			}
			else if (connect())
			{
				open();
			}
		}

		public void ConnectAsync()
		{
			string text = checkIfCanConnect();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in connecting.", null);
				return;
			}
			Func<bool> connector = connect;
			connector.BeginInvoke(delegate(IAsyncResult ar)
			{
				if (connector.EndInvoke(ar))
				{
					open();
				}
			}, null);
		}

		public bool Ping()
		{
			byte[] frameAsBytes = ((!_client) ? WebSocketFrame.EmptyUnmaskPingBytes : WebSocketFrame.CreatePingFrame(true).ToByteArray());
			return Ping(frameAsBytes, _waitTime);
		}

		public bool Ping(string message)
		{
			if (message == null || message.Length == 0)
			{
				return Ping();
			}
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			string text = bytes.CheckIfValidControlData("message");
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the ping.", null);
				return false;
			}
			return Ping(WebSocketFrame.CreatePingFrame(bytes, _client).ToByteArray(), _waitTime);
		}

		public void Send(byte[] data)
		{
			string text = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
			}
			else
			{
				send(Opcode.Binary, new MemoryStream(data));
			}
		}

		public void Send(FileInfo file)
		{
			string text = _readyState.CheckIfOpen() ?? file.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
			}
			else
			{
				send(Opcode.Binary, file.OpenRead());
			}
		}

		public void Send(string data)
		{
			string text = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
			}
			else
			{
				send(Opcode.Text, new MemoryStream(Encoding.UTF8.GetBytes(data)));
			}
		}

		public void SendAsync(byte[] data, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
			}
			else
			{
				sendAsync(Opcode.Binary, new MemoryStream(data), completed);
			}
		}

		public void SendAsync(FileInfo file, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? file.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
			}
			else
			{
				sendAsync(Opcode.Binary, file.OpenRead(), completed);
			}
		}

		public void SendAsync(string data, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
			}
			else
			{
				sendAsync(Opcode.Text, new MemoryStream(Encoding.UTF8.GetBytes(data)), completed);
			}
		}

		public void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			string text = _readyState.CheckIfOpen() ?? stream.CheckIfCanRead() ?? ((length >= 1) ? null : "'length' is less than 1.");
			if (text != null)
			{
				_logger.Error(text);
				error("An error has occurred in sending the data.", null);
				return;
			}
			stream.ReadBytesAsync(length, delegate(byte[] data)
			{
				int num = data.Length;
				if (num == 0)
				{
					_logger.Error("The data cannot be read from 'stream'.");
					error("An error has occurred in sending the data.", null);
				}
				else
				{
					if (num < length)
					{
						_logger.Warn(string.Format("The data with 'length' cannot be read from 'stream'.\nexpected: {0} actual: {1}", length, num));
					}
					bool obj = send(Opcode.Binary, new MemoryStream(data));
					if (completed != null)
					{
						completed(obj);
					}
				}
			}, delegate(Exception ex)
			{
				_logger.Fatal(ex.ToString());
				error("An exception has occurred while sending the data.", ex);
			});
		}

		public void SetCookie(Cookie cookie)
		{
			lock (_forConn)
			{
				string text = checkIfAvailable(false, false) ?? ((cookie != null) ? null : "'cookie' is null.");
				if (text != null)
				{
					_logger.Error(text);
					error("An error has occurred in setting the cookie.", null);
					return;
				}
				lock (_cookies.SyncRoot)
				{
					_cookies.SetOrRemove(cookie);
				}
			}
		}

		public void SetCredentials(string username, string password, bool preAuth)
		{
			lock (_forConn)
			{
				string text = checkIfAvailable(false, false);
				if (text == null)
				{
					if (username.IsNullOrEmpty())
					{
						_credentials = null;
						_preAuth = false;
						_logger.Warn("The credentials were set back to the default.");
						return;
					}
					text = ((username.Contains(':') || !username.IsText()) ? "'username' contains an invalid character." : ((password.IsNullOrEmpty() || password.IsText()) ? null : "'password' contains an invalid character."));
				}
				if (text != null)
				{
					_logger.Error(text);
					error("An error has occurred in setting the credentials.", null);
				}
				else
				{
					_credentials = new NetworkCredential(username, password, _uri.PathAndQuery);
					_preAuth = preAuth;
				}
			}
		}

		public void SetProxy(string url, string username, string password)
		{
			lock (_forConn)
			{
				string text = checkIfAvailable(false, false);
				if (text == null)
				{
					if (url.IsNullOrEmpty())
					{
						_proxyUri = null;
						_proxyCredentials = null;
						_logger.Warn("The proxy url and credentials were set back to the default.");
						return;
					}
					Uri result;
					if (!Uri.TryCreate(url, UriKind.Absolute, out result) || result.Scheme != "http" || result.Segments.Length > 1)
					{
						text = "The syntax of the proxy url must be 'http://<host>[:<port>]'.";
					}
					else
					{
						_proxyUri = result;
						if (username.IsNullOrEmpty())
						{
							_proxyCredentials = null;
							_logger.Warn("The proxy credentials were set back to the default.");
							return;
						}
						text = ((username.Contains(':') || !username.IsText()) ? "'username' contains an invalid character." : ((password.IsNullOrEmpty() || password.IsText()) ? null : "'password' contains an invalid character."));
					}
				}
				if (text != null)
				{
					_logger.Error(text);
					error("An error has occurred in setting the proxy.", null);
				}
				else
				{
					_proxyCredentials = new NetworkCredential(username, password, string.Format("{0}:{1}", _uri.DnsSafeHost, _uri.Port));
				}
			}
		}
	}
}
