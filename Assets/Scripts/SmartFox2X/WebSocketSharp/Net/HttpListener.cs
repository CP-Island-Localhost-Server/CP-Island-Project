using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public sealed class HttpListener : IDisposable
	{
		private AuthenticationSchemes _authSchemes;

		private Func<HttpListenerRequest, AuthenticationSchemes> _authSchemeSelector;

		private string _certFolderPath;

		private Dictionary<HttpConnection, HttpConnection> _connections;

		private object _connectionsSync;

		private List<HttpListenerContext> _ctxQueue;

		private object _ctxQueueSync;

		private Dictionary<HttpListenerContext, HttpListenerContext> _ctxRegistry;

		private object _ctxRegistrySync;

		private Func<IIdentity, NetworkCredential> _credFinder;

		private bool _disposed;

		private bool _ignoreWriteExceptions;

		private bool _listening;

		private Logger _logger;

		private HttpListenerPrefixCollection _prefixes;

		private string _realm;

		private bool _reuseAddress;

		private ServerSslConfiguration _sslConfig;

		private List<HttpListenerAsyncResult> _waitQueue;

		private object _waitQueueSync;

		internal bool IsDisposed
		{
			get
			{
				return _disposed;
			}
		}

		internal bool ReuseAddress
		{
			get
			{
				return _reuseAddress;
			}
			set
			{
				_reuseAddress = value;
			}
		}

		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				CheckDisposed();
				return _authSchemes;
			}
			set
			{
				CheckDisposed();
				_authSchemes = value;
			}
		}

		public Func<HttpListenerRequest, AuthenticationSchemes> AuthenticationSchemeSelector
		{
			get
			{
				CheckDisposed();
				return _authSchemeSelector;
			}
			set
			{
				CheckDisposed();
				_authSchemeSelector = value;
			}
		}

		public string CertificateFolderPath
		{
			get
			{
				CheckDisposed();
				return _certFolderPath;
			}
			set
			{
				CheckDisposed();
				_certFolderPath = value;
			}
		}

		public bool IgnoreWriteExceptions
		{
			get
			{
				CheckDisposed();
				return _ignoreWriteExceptions;
			}
			set
			{
				CheckDisposed();
				_ignoreWriteExceptions = value;
			}
		}

		public bool IsListening
		{
			get
			{
				return _listening;
			}
		}

		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public Logger Log
		{
			get
			{
				return _logger;
			}
		}

		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				CheckDisposed();
				return _prefixes;
			}
		}

		public string Realm
		{
			get
			{
				CheckDisposed();
				return (_realm == null || _realm.Length <= 0) ? (_realm = "SECRET AREA") : _realm;
			}
			set
			{
				CheckDisposed();
				_realm = value;
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				CheckDisposed();
				return _sslConfig ?? (_sslConfig = new ServerSslConfiguration(null));
			}
			set
			{
				CheckDisposed();
				_sslConfig = value;
			}
		}

		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public Func<IIdentity, NetworkCredential> UserCredentialsFinder
		{
			get
			{
				CheckDisposed();
				return (IIdentity id) => (NetworkCredential)null;
			}
			set
			{
				CheckDisposed();
				_credFinder = value;
			}
		}

		public HttpListener()
		{
			_authSchemes = AuthenticationSchemes.Anonymous;
			_connections = new Dictionary<HttpConnection, HttpConnection>();
			_connectionsSync = ((ICollection)_connections).SyncRoot;
			_ctxQueue = new List<HttpListenerContext>();
			_ctxQueueSync = ((ICollection)_ctxQueue).SyncRoot;
			_ctxRegistry = new Dictionary<HttpListenerContext, HttpListenerContext>();
			_ctxRegistrySync = ((ICollection)_ctxRegistry).SyncRoot;
			_logger = new Logger();
			_prefixes = new HttpListenerPrefixCollection(this);
			_waitQueue = new List<HttpListenerAsyncResult>();
			_waitQueueSync = ((ICollection)_waitQueue).SyncRoot;
		}

		void IDisposable.Dispose()
		{
			if (!_disposed)
			{
				close(true);
				_disposed = true;
			}
		}

		private void cleanup(bool force)
		{
			lock (_ctxRegistrySync)
			{
				if (!force)
				{
					sendServiceUnavailable();
				}
				cleanupContextRegistry();
				cleanupConnections();
				cleanupWaitQueue();
			}
		}

		private void cleanupConnections()
		{
			lock (_connectionsSync)
			{
				if (_connections.Count != 0)
				{
					Dictionary<HttpConnection, HttpConnection>.KeyCollection keys = _connections.Keys;
					HttpConnection[] array = new HttpConnection[keys.Count];
					keys.CopyTo(array, 0);
					_connections.Clear();
					for (int num = array.Length - 1; num >= 0; num--)
					{
						array[num].Close(true);
					}
				}
			}
		}

		private void cleanupContextRegistry()
		{
			lock (_ctxRegistrySync)
			{
				if (_ctxRegistry.Count != 0)
				{
					Dictionary<HttpListenerContext, HttpListenerContext>.KeyCollection keys = _ctxRegistry.Keys;
					HttpListenerContext[] array = new HttpListenerContext[keys.Count];
					keys.CopyTo(array, 0);
					_ctxRegistry.Clear();
					for (int num = array.Length - 1; num >= 0; num--)
					{
						array[num].Connection.Close(true);
					}
				}
			}
		}

		private void cleanupWaitQueue()
		{
			lock (_waitQueueSync)
			{
				if (_waitQueue.Count == 0)
				{
					return;
				}
				ObjectDisposedException exception = new ObjectDisposedException(GetType().ToString());
				foreach (HttpListenerAsyncResult item in _waitQueue)
				{
					item.Complete(exception);
				}
				_waitQueue.Clear();
			}
		}

		private void close(bool force)
		{
			EndPointManager.RemoveListener(this);
			cleanup(force);
		}

		private HttpListenerContext getContextFromQueue()
		{
			if (_ctxQueue.Count == 0)
			{
				return null;
			}
			HttpListenerContext result = _ctxQueue[0];
			_ctxQueue.RemoveAt(0);
			return result;
		}

		private void sendServiceUnavailable()
		{
			lock (_ctxQueueSync)
			{
				if (_ctxQueue.Count != 0)
				{
					HttpListenerContext[] array = _ctxQueue.ToArray();
					_ctxQueue.Clear();
					HttpListenerContext[] array2 = array;
					foreach (HttpListenerContext httpListenerContext in array2)
					{
						HttpListenerResponse response = httpListenerContext.Response;
						response.StatusCode = 503;
						response.Close();
					}
				}
			}
		}

		internal void AddConnection(HttpConnection connection)
		{
			lock (_connectionsSync)
			{
				_connections[connection] = connection;
			}
		}

		internal bool Authenticate(HttpListenerContext context)
		{
			AuthenticationSchemes authenticationSchemes = SelectAuthenticationScheme(context);
			switch (authenticationSchemes)
			{
			case AuthenticationSchemes.Anonymous:
				return true;
			default:
				context.Response.Close(HttpStatusCode.Forbidden);
				return false;
			case AuthenticationSchemes.Digest:
			case AuthenticationSchemes.Basic:
			{
				string realm = Realm;
				HttpListenerRequest request = context.Request;
				IPrincipal principal = HttpUtility.CreateUser(request.Headers["Authorization"], authenticationSchemes, realm, request.HttpMethod, UserCredentialsFinder);
				if (principal != null && principal.Identity.IsAuthenticated)
				{
					context.User = principal;
					return true;
				}
				if (authenticationSchemes == AuthenticationSchemes.Basic)
				{
					context.Response.CloseWithAuthChallenge(AuthenticationChallenge.CreateBasicChallenge(realm).ToBasicString());
				}
				if (authenticationSchemes == AuthenticationSchemes.Digest)
				{
					context.Response.CloseWithAuthChallenge(AuthenticationChallenge.CreateDigestChallenge(realm).ToDigestString());
				}
				return false;
			}
			}
		}

		internal HttpListenerAsyncResult BeginGetContext(HttpListenerAsyncResult asyncResult)
		{
			CheckDisposed();
			if (_prefixes.Count == 0)
			{
				throw new InvalidOperationException("The listener has no URI prefix on which listens.");
			}
			if (!_listening)
			{
				throw new InvalidOperationException("The listener hasn't been started.");
			}
			lock (_waitQueueSync)
			{
				lock (_ctxQueueSync)
				{
					HttpListenerContext contextFromQueue = getContextFromQueue();
					if (contextFromQueue != null)
					{
						asyncResult.Complete(contextFromQueue, true);
						return asyncResult;
					}
				}
				_waitQueue.Add(asyncResult);
				return asyncResult;
			}
		}

		internal void CheckDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}

		internal void RegisterContext(HttpListenerContext context)
		{
			lock (_ctxRegistrySync)
			{
				_ctxRegistry[context] = context;
			}
			HttpListenerAsyncResult httpListenerAsyncResult = null;
			lock (_waitQueueSync)
			{
				if (_waitQueue.Count == 0)
				{
					lock (_ctxQueueSync)
					{
						_ctxQueue.Add(context);
					}
				}
				else
				{
					httpListenerAsyncResult = _waitQueue[0];
					_waitQueue.RemoveAt(0);
				}
			}
			if (httpListenerAsyncResult != null)
			{
				httpListenerAsyncResult.Complete(context);
			}
		}

		internal void RemoveConnection(HttpConnection connection)
		{
			lock (_connectionsSync)
			{
				_connections.Remove(connection);
			}
		}

		internal AuthenticationSchemes SelectAuthenticationScheme(HttpListenerContext context)
		{
			return (AuthenticationSchemeSelector == null) ? _authSchemes : AuthenticationSchemeSelector(context.Request);
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			lock (_ctxRegistrySync)
			{
				_ctxRegistry.Remove(context);
			}
			lock (_ctxQueueSync)
			{
				int num = _ctxQueue.IndexOf(context);
				if (num >= 0)
				{
					_ctxQueue.RemoveAt(num);
				}
			}
		}

		public void Abort()
		{
			if (!_disposed)
			{
				close(true);
				_disposed = true;
			}
		}

		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			return BeginGetContext(new HttpListenerAsyncResult(callback, state));
		}

		public void Close()
		{
			if (!_disposed)
			{
				close(false);
				_disposed = true;
			}
		}

		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			HttpListenerAsyncResult httpListenerAsyncResult = asyncResult as HttpListenerAsyncResult;
			if (httpListenerAsyncResult == null)
			{
				throw new ArgumentException("A wrong IAsyncResult.", "asyncResult");
			}
			if (httpListenerAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("This IAsyncResult cannot be reused.");
			}
			httpListenerAsyncResult.EndCalled = true;
			if (!httpListenerAsyncResult.IsCompleted)
			{
				httpListenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			return httpListenerAsyncResult.GetContext();
		}

		public HttpListenerContext GetContext()
		{
			HttpListenerAsyncResult httpListenerAsyncResult = BeginGetContext(new HttpListenerAsyncResult(null, null));
			httpListenerAsyncResult.InGet = true;
			return EndGetContext(httpListenerAsyncResult);
		}

		public void Start()
		{
			CheckDisposed();
			if (!_listening)
			{
				EndPointManager.AddListener(this);
				_listening = true;
			}
		}

		public void Stop()
		{
			CheckDisposed();
			if (_listening)
			{
				_listening = false;
				EndPointManager.RemoveListener(this);
				sendServiceUnavailable();
			}
		}
	}
}
