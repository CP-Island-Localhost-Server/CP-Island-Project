using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;

namespace WebSocketSharp.Server
{
	public class WebSocketServiceManager
	{
		private volatile bool _clean;

		private Dictionary<string, WebSocketServiceHost> _hosts;

		private Logger _logger;

		private volatile ServerState _state;

		private object _sync;

		private TimeSpan _waitTime;

		public int Count
		{
			get
			{
				lock (_sync)
				{
					return _hosts.Count;
				}
			}
		}

		public IEnumerable<WebSocketServiceHost> Hosts
		{
			get
			{
				lock (_sync)
				{
					return _hosts.Values.ToList();
				}
			}
		}

		public WebSocketServiceHost this[string path]
		{
			get
			{
				WebSocketServiceHost host;
				TryGetServiceHost(path, out host);
				return host;
			}
		}

		public bool KeepClean
		{
			get
			{
				return _clean;
			}
			internal set
			{
				lock (_sync)
				{
					if (!(value ^ _clean))
					{
						return;
					}
					_clean = value;
					foreach (WebSocketServiceHost value2 in _hosts.Values)
					{
						value2.KeepClean = value;
					}
				}
			}
		}

		public IEnumerable<string> Paths
		{
			get
			{
				lock (_sync)
				{
					return _hosts.Keys.ToList();
				}
			}
		}

		public int SessionCount
		{
			get
			{
				int num = 0;
				foreach (WebSocketServiceHost host in Hosts)
				{
					if (_state != ServerState.Start)
					{
						break;
					}
					num += host.Sessions.Count;
				}
				return num;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return _waitTime;
			}
			internal set
			{
				lock (_sync)
				{
					if (value == _waitTime)
					{
						return;
					}
					_waitTime = value;
					foreach (WebSocketServiceHost value2 in _hosts.Values)
					{
						value2.WaitTime = value;
					}
				}
			}
		}

		internal WebSocketServiceManager()
			: this(new Logger())
		{
		}

		internal WebSocketServiceManager(Logger logger)
		{
			_logger = logger;
			_clean = true;
			_hosts = new Dictionary<string, WebSocketServiceHost>();
			_state = ServerState.Ready;
			_sync = ((ICollection)_hosts).SyncRoot;
			_waitTime = TimeSpan.FromSeconds(1.0);
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> dictionary = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				foreach (WebSocketServiceHost host in Hosts)
				{
					if (_state != ServerState.Start)
					{
						break;
					}
					host.Sessions.Broadcast(opcode, data, dictionary);
				}
				if (completed != null)
				{
					completed();
				}
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
			}
			finally
			{
				dictionary.Clear();
			}
		}

		private void broadcast(Opcode opcode, Stream stream, Action completed)
		{
			Dictionary<CompressionMethod, Stream> dictionary = new Dictionary<CompressionMethod, Stream>();
			try
			{
				foreach (WebSocketServiceHost host in Hosts)
				{
					if (_state != ServerState.Start)
					{
						break;
					}
					host.Sessions.Broadcast(opcode, stream, dictionary);
				}
				if (completed != null)
				{
					completed();
				}
			}
			catch (Exception ex)
			{
				_logger.Fatal(ex.ToString());
			}
			finally
			{
				foreach (Stream value in dictionary.Values)
				{
					value.Dispose();
				}
				dictionary.Clear();
			}
		}

		private void broadcastAsync(Opcode opcode, byte[] data, Action completed)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				broadcast(opcode, data, completed);
			});
		}

		private void broadcastAsync(Opcode opcode, Stream stream, Action completed)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				broadcast(opcode, stream, completed);
			});
		}

		private Dictionary<string, Dictionary<string, bool>> broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, Dictionary<string, bool>> dictionary = new Dictionary<string, Dictionary<string, bool>>();
			foreach (WebSocketServiceHost host in Hosts)
			{
				if (_state != ServerState.Start)
				{
					break;
				}
				dictionary.Add(host.Path, host.Sessions.Broadping(frameAsBytes, timeout));
			}
			return dictionary;
		}

		internal void Add<TBehavior>(string path, Func<TBehavior> initializer) where TBehavior : WebSocketBehavior
		{
			lock (_sync)
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				WebSocketServiceHost value;
				if (_hosts.TryGetValue(path, out value))
				{
					_logger.Error("A WebSocket service with the specified path already exists.\npath: " + path);
					return;
				}
				value = new WebSocketServiceHost<TBehavior>(path, initializer, _logger);
				if (!_clean)
				{
					value.KeepClean = false;
				}
				if (_waitTime != value.WaitTime)
				{
					value.WaitTime = _waitTime;
				}
				if (_state == ServerState.Start)
				{
					value.Start();
				}
				_hosts.Add(path, value);
			}
		}

		internal bool InternalTryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			bool flag;
			lock (_sync)
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				flag = _hosts.TryGetValue(path, out host);
			}
			if (!flag)
			{
				_logger.Error("A WebSocket service with the specified path isn't found.\npath: " + path);
			}
			return flag;
		}

		internal bool Remove(string path)
		{
			WebSocketServiceHost value;
			lock (_sync)
			{
				path = HttpUtility.UrlDecode(path).TrimEndSlash();
				if (!_hosts.TryGetValue(path, out value))
				{
					_logger.Error("A WebSocket service with the specified path isn't found.\npath: " + path);
					return false;
				}
				_hosts.Remove(path);
			}
			if (value.State == ServerState.Start)
			{
				value.Stop(1001, null);
			}
			return true;
		}

		internal void Start()
		{
			lock (_sync)
			{
				foreach (WebSocketServiceHost value in _hosts.Values)
				{
					value.Start();
				}
				_state = ServerState.Start;
			}
		}

		internal void Stop(CloseEventArgs e, bool send, bool wait)
		{
			lock (_sync)
			{
				_state = ServerState.ShuttingDown;
				byte[] frameAsBytes = ((!send) ? null : WebSocketFrame.CreateCloseFrame(e.PayloadData, false).ToByteArray());
				TimeSpan timeout = ((!wait) ? TimeSpan.Zero : _waitTime);
				foreach (WebSocketServiceHost value in _hosts.Values)
				{
					value.Sessions.Stop(e, frameAsBytes, timeout);
				}
				_hosts.Clear();
				_state = ServerState.Stop;
			}
		}

		public void Broadcast(byte[] data)
		{
			string text = _state.CheckIfStart() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
			}
			else if (data.LongLength <= 2147483633)
			{
				broadcast(Opcode.Binary, data, null);
			}
			else
			{
				broadcast(Opcode.Binary, new MemoryStream(data), null);
			}
		}

		public void Broadcast(string data)
		{
			string text = _state.CheckIfStart() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			if (bytes.LongLength <= 2147483633)
			{
				broadcast(Opcode.Text, bytes, null);
			}
			else
			{
				broadcast(Opcode.Text, new MemoryStream(bytes), null);
			}
		}

		public void BroadcastAsync(byte[] data, Action completed)
		{
			string text = _state.CheckIfStart() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
			}
			else if (data.LongLength <= 2147483633)
			{
				broadcastAsync(Opcode.Binary, data, completed);
			}
			else
			{
				broadcastAsync(Opcode.Binary, new MemoryStream(data), completed);
			}
		}

		public void BroadcastAsync(string data, Action completed)
		{
			string text = _state.CheckIfStart() ?? data.CheckIfValidSendData();
			if (text != null)
			{
				_logger.Error(text);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			if (bytes.LongLength <= 2147483633)
			{
				broadcastAsync(Opcode.Text, bytes, completed);
			}
			else
			{
				broadcastAsync(Opcode.Text, new MemoryStream(bytes), completed);
			}
		}

		public void BroadcastAsync(Stream stream, int length, Action completed)
		{
			string text = _state.CheckIfStart() ?? stream.CheckIfCanRead() ?? ((length >= 1) ? null : "'length' is less than 1.");
			if (text != null)
			{
				_logger.Error(text);
				return;
			}
			stream.ReadBytesAsync(length, delegate(byte[] data)
			{
				int num = data.Length;
				if (num == 0)
				{
					_logger.Error("The data cannot be read from 'stream'.");
				}
				else
				{
					if (num < length)
					{
						_logger.Warn(string.Format("The data with 'length' cannot be read from 'stream'.\nexpected: {0} actual: {1}", length, num));
					}
					if (num <= 2147483633)
					{
						broadcast(Opcode.Binary, data, completed);
					}
					else
					{
						broadcast(Opcode.Binary, new MemoryStream(data), completed);
					}
				}
			}, delegate(Exception ex)
			{
				_logger.Fatal(ex.ToString());
			});
		}

		public Dictionary<string, Dictionary<string, bool>> Broadping()
		{
			string text = _state.CheckIfStart();
			if (text != null)
			{
				_logger.Error(text);
				return null;
			}
			return broadping(WebSocketFrame.EmptyUnmaskPingBytes, _waitTime);
		}

		public Dictionary<string, Dictionary<string, bool>> Broadping(string message)
		{
			if (message == null || message.Length == 0)
			{
				return Broadping();
			}
			byte[] data = null;
			string text = _state.CheckIfStart() ?? (data = Encoding.UTF8.GetBytes(message)).CheckIfValidControlData("message");
			if (text != null)
			{
				_logger.Error(text);
				return null;
			}
			return broadping(WebSocketFrame.CreatePingFrame(data, false).ToByteArray(), _waitTime);
		}

		public bool TryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			string text = _state.CheckIfStart() ?? path.CheckIfValidServicePath();
			if (text != null)
			{
				_logger.Error(text);
				host = null;
				return false;
			}
			return InternalTryGetServiceHost(path, out host);
		}
	}
}
