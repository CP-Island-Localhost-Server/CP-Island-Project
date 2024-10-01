using System;
using System.Net.Sockets;
using Sfs2X.Bitswarm;

namespace Sfs2X.Core.Sockets
{
	public class WebSocketLayer : BaseSocketLayer, ISocketLayer
	{
		private WebSocketHelper wsh;

		private bool useWSS;

		private ConnectionDelegate onConnect;

		private ConnectionDelegate onDisconnect;

		private OnStringDataDelegate onData = null;

		private OnErrorDelegate onError = null;

		public bool IsConnected
		{
			get
			{
				return base.State == States.Connected;
			}
		}

		public bool RequiresConnection
		{
			get
			{
				return true;
			}
		}

		public ConnectionDelegate OnConnect
		{
			get
			{
				return onConnect;
			}
			set
			{
				onConnect = value;
			}
		}

		public ConnectionDelegate OnDisconnect
		{
			get
			{
				return onDisconnect;
			}
			set
			{
				onDisconnect = value;
			}
		}

		public OnDataDelegate OnData
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public OnStringDataDelegate OnStringData
		{
			get
			{
				return onData;
			}
			set
			{
				onData = value;
			}
		}

		public OnErrorDelegate OnError
		{
			get
			{
				return onError;
			}
			set
			{
				onError = value;
			}
		}

		public WebSocketLayer(WebSocketClient wsc, bool useWSS)
		{
			socketClient = wsc;
			log = wsc.Log;
			this.useWSS = useWSS;
			InitStates();
		}

		private void LogWarn(string msg)
		{
			if (log != null)
			{
				log.Warn("[WebSocketLayer] " + msg);
			}
		}

		private void LogError(string msg)
		{
			if (log != null)
			{
				log.Error("[WebSocketLayer] " + msg);
			}
		}

		public void Connect(string host, int port)
		{
			if (base.State != 0)
			{
				LogWarn("Call to Connect method ignored, as the websocket is already connected");
				return;
			}
			string uriString = "ws" + ((!useWSS) ? "" : "s") + "://" + host + ":" + port + "/websocket";
			wsh = new WebSocketHelper(new Uri(uriString), log);
			fsm.ApplyTransition(Transitions.StartConnect);
			wsh.Connect();
		}

		public void Disconnect()
		{
			Disconnect(null);
		}

		public void Disconnect(string reason)
		{
			if (base.State != States.Connected)
			{
				LogWarn("Calling disconnect when the socket is not connected");
				return;
			}
			isDisconnecting = true;
			wsh.Close();
			HandleDisconnection(reason);
			isDisconnecting = false;
		}

		public void Write(byte[] data)
		{
			LogError("Method Write(byte[] data) is not implemented because it is reserved to standard socket communication");
			throw new NotImplementedException();
		}

		public void Write(string data)
		{
			if (base.State != States.Connected)
			{
				LogError("Trying to write to disconnected websocket");
			}
			else
			{
				wsh.Send(data);
			}
		}

		public void Kill()
		{
		}

		public void ProcessState()
		{
			if (base.State == States.Connecting)
			{
				if (wsh.Error != null)
				{
					string err = "Connection error: " + wsh.Error.Message + ((wsh.Error.Exception == null) ? null : (" " + wsh.Error.Exception.StackTrace));
					HandleError(err, wsh.Error.Exception);
				}
				else if (wsh.IsConnected)
				{
					fsm.ApplyTransition(Transitions.ConnectionSuccess);
					CallOnConnect();
				}
			}
			else
			{
				if (base.State != States.Connected)
				{
					return;
				}
				if (wsh.Error != null)
				{
					string err2 = "Communication error: " + wsh.Error.Message + ((wsh.Error.Exception == null) ? null : (" " + wsh.Error.Exception.StackTrace));
					HandleError(err2, wsh.Error.Exception);
				}
				else
				{
					string data;
					while ((data = wsh.ReceiveString()) != null)
					{
						CallOnData(data);
					}
				}
			}
		}

		private void HandleError(string err, Exception e)
		{
			SocketError se = SocketError.SocketError;
			if (e != null && e is SocketException)
			{
				se = (e as SocketException).SocketErrorCode;
			}
			fsm.ApplyTransition(Transitions.ConnectionFailure);
			if (!isDisconnecting)
			{
				LogError(err);
				onError(err, se);
			}
			HandleDisconnection();
		}

		private void HandleDisconnection()
		{
			HandleDisconnection(null);
		}

		private void HandleDisconnection(string reason)
		{
			if (base.State != 0)
			{
				fsm.ApplyTransition(Transitions.Disconnect);
				if (reason == null)
				{
					CallOnDisconnect();
				}
			}
		}

		private void CallOnConnect()
		{
			if (onConnect != null)
			{
				onConnect();
			}
		}

		private void CallOnDisconnect()
		{
			if (onDisconnect != null)
			{
				onDisconnect();
			}
		}

		private void CallOnData(string data)
		{
			if (onData != null)
			{
				onData(data);
			}
		}

		private void CallOnError(string msg, SocketError se)
		{
			if (onError != null)
			{
				onError(msg, se);
			}
		}
	}
}
