using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

namespace WebSocketSharp.Server
{
	public class WebSocketSessionManager
	{
		private volatile bool _clean;

		private object _forSweep;

		private Logger _logger;

		private Dictionary<string, IWebSocketSession> _sessions;

		private volatile ServerState _state;

		private volatile bool _sweeping;

		private System.Timers.Timer _sweepTimer;

		private object _sync;

		private TimeSpan _waitTime;

		internal ServerState State
		{
			get
			{
				return _state;
			}
		}

		public IEnumerable<string> ActiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> res in Broadping(WebSocketFrame.EmptyUnmaskPingBytes, _waitTime))
				{
					if (res.Value)
					{
						yield return res.Key;
					}
				}
			}
		}

		public int Count
		{
			get
			{
				lock (_sync)
				{
					return _sessions.Count;
				}
			}
		}

		public IEnumerable<string> IDs
		{
			get
			{
				if (_state == ServerState.ShuttingDown)
				{
					return new string[0];
				}
				lock (_sync)
				{
					return _sessions.Keys.ToList();
				}
			}
		}

		public IEnumerable<string> InactiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> res in Broadping(WebSocketFrame.EmptyUnmaskPingBytes, _waitTime))
				{
					if (!res.Value)
					{
						yield return res.Key;
					}
				}
			}
		}

		public IWebSocketSession this[string id]
		{
			get
			{
				IWebSocketSession session;
				TryGetSession(id, out session);
				return session;
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
				if (value ^ _clean)
				{
					_clean = value;
					if (_state == ServerState.Start)
					{
						_sweepTimer.Enabled = value;
					}
				}
			}
		}

		public IEnumerable<IWebSocketSession> Sessions
		{
			get
			{
				if (_state == ServerState.ShuttingDown)
				{
					return new IWebSocketSession[0];
				}
				lock (_sync)
				{
					return _sessions.Values.ToList();
				}
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
				if (value == _waitTime)
				{
					return;
				}
				_waitTime = value;
				foreach (IWebSocketSession session in Sessions)
				{
					session.Context.WebSocket.WaitTime = value;
				}
			}
		}

		internal WebSocketSessionManager()
			: this(new Logger())
		{
		}

		internal WebSocketSessionManager(Logger logger)
		{
			_logger = logger;
			_clean = true;
			_forSweep = new object();
			_sessions = new Dictionary<string, IWebSocketSession>();
			_state = ServerState.Ready;
			_sync = ((ICollection)_sessions).SyncRoot;
			_waitTime = TimeSpan.FromSeconds(1.0);
			setSweepTimer(60000.0);
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> dictionary = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				Broadcast(opcode, data, dictionary);
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
				Broadcast(opcode, stream, dictionary);
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

		private static string createID()
		{
			return Guid.NewGuid().ToString("N");
		}

		private void setSweepTimer(double interval)
		{
			_sweepTimer = new System.Timers.Timer(interval);
			_sweepTimer.Elapsed += delegate
			{
				Sweep();
			};
		}

		private bool tryGetSession(string id, out IWebSocketSession session)
		{
			bool flag;
			lock (_sync)
			{
				flag = _sessions.TryGetValue(id, out session);
			}
			if (!flag)
			{
				_logger.Error("A session with the specified ID isn't found:\n  ID: " + id);
			}
			return flag;
		}

		internal string Add(IWebSocketSession session)
		{
			lock (_sync)
			{
				if (_state != ServerState.Start)
				{
					return null;
				}
				string text = createID();
				_sessions.Add(text, session);
				return text;
			}
		}

		internal void Broadcast(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					break;
				}
				session.Context.WebSocket.Send(opcode, data, cache);
			}
		}

		internal void Broadcast(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					break;
				}
				session.Context.WebSocket.Send(opcode, stream, cache);
			}
		}

		internal Dictionary<string, bool> Broadping(byte[] frameAsBytes, TimeSpan timeout)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (IWebSocketSession session in Sessions)
			{
				if (_state != ServerState.Start)
				{
					break;
				}
				dictionary.Add(session.ID, session.Context.WebSocket.Ping(frameAsBytes, timeout));
			}
			return dictionary;
		}

		internal bool Remove(string id)
		{
			lock (_sync)
			{
				return _sessions.Remove(id);
			}
		}

		internal void Start()
		{
			lock (_sync)
			{
				_sweepTimer.Enabled = _clean;
				_state = ServerState.Start;
			}
		}

		internal void Stop(CloseEventArgs e, byte[] frameAsBytes, TimeSpan timeout)
		{
			lock (_sync)
			{
				_state = ServerState.ShuttingDown;
				_sweepTimer.Enabled = false;
				foreach (IWebSocketSession item in _sessions.Values.ToList())
				{
					item.Context.WebSocket.Close(e, frameAsBytes, timeout);
				}
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
						_logger.Warn(string.Format("The data with 'length' cannot be read from 'stream':\n  expected: {0}\n  actual: {1}", length, num));
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

		public Dictionary<string, bool> Broadping()
		{
			string text = _state.CheckIfStart();
			if (text != null)
			{
				_logger.Error(text);
				return null;
			}
			return Broadping(WebSocketFrame.EmptyUnmaskPingBytes, _waitTime);
		}

		public Dictionary<string, bool> Broadping(string message)
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
			return Broadping(WebSocketFrame.CreatePingFrame(data, false).ToByteArray(), _waitTime);
		}

		public void CloseSession(string id)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.Close();
			}
		}

		public void CloseSession(string id, ushort code, string reason)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.Close(code, reason);
			}
		}

		public void CloseSession(string id, CloseStatusCode code, string reason)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.Close(code, reason);
			}
		}

		public bool PingTo(string id)
		{
			IWebSocketSession session;
			return TryGetSession(id, out session) && session.Context.WebSocket.Ping();
		}

		public bool PingTo(string message, string id)
		{
			IWebSocketSession session;
			return TryGetSession(id, out session) && session.Context.WebSocket.Ping(message);
		}

		public void SendTo(byte[] data, string id)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.Send(data);
			}
		}

		public void SendTo(string data, string id)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.Send(data);
			}
		}

		public void SendToAsync(byte[] data, string id, Action<bool> completed)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.SendAsync(data, completed);
			}
		}

		public void SendToAsync(string data, string id, Action<bool> completed)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.SendAsync(data, completed);
			}
		}

		public void SendToAsync(Stream stream, int length, string id, Action<bool> completed)
		{
			IWebSocketSession session;
			if (TryGetSession(id, out session))
			{
				session.Context.WebSocket.SendAsync(stream, length, completed);
			}
		}

		public void Sweep()
		{
			if (_state != ServerState.Start || _sweeping || Count == 0)
			{
				return;
			}
			lock (_forSweep)
			{
				_sweeping = true;
				foreach (string inactiveID in InactiveIDs)
				{
					if (_state != ServerState.Start)
					{
						break;
					}
					lock (_sync)
					{
						IWebSocketSession value;
						if (_sessions.TryGetValue(inactiveID, out value))
						{
							switch (value.State)
							{
							case WebSocketState.Open:
								value.Context.WebSocket.Close(CloseStatusCode.ProtocolError);
								break;
							case WebSocketState.Closing:
								break;
							default:
								_sessions.Remove(inactiveID);
								break;
							}
						}
					}
				}
				_sweeping = false;
			}
		}

		public bool TryGetSession(string id, out IWebSocketSession session)
		{
			string text = _state.CheckIfStart() ?? id.CheckIfValidSessionID();
			if (text != null)
			{
				_logger.Error(text);
				session = null;
				return false;
			}
			return tryGetSession(id, out session);
		}
	}
}
