using System;
using System.Collections;
using System.Threading;
using Sfs2X.Core;
using Sfs2X.Http;
using Sfs2X.Logging;
using Sfs2X.Util;

namespace Sfs2X.Bitswarm.BBox
{
	public class BBClient : IDispatchable
	{
		public const string BB_SERVLET = "BlueBox/BlueBox.do";

		private const string BB_DEFAULT_HOST = "localhost";

		private const int BB_DEFAULT_PORT = 8080;

		private const string BB_NULL = "null";

		private const string CMD_CONNECT = "connect";

		private const string CMD_POLL = "poll";

		private const string CMD_DATA = "data";

		private const string CMD_DISCONNECT = "disconnect";

		private const string ERR_INVALID_SESSION = "err01";

		private const string SFS_HTTP = "sfsHttp";

		private const char SEP = '|';

		private const int MIN_POLL_SPEED = 50;

		private const int MAX_POLL_SPEED = 5000;

		private const int DEFAULT_POLL_SPEED = 300;

		private bool isConnected = false;

		private string host = "localhost";

		private int port = 8080;

		private string bbUrl;

		private bool debug;

		private string sessId;

		private int pollSpeed = 300;

		private EventDispatcher dispatcher;

		private Logger log;

		private Timer pollTimer = null;

		public bool IsConnected
		{
			get
			{
				return sessId != null;
			}
		}

		public bool IsDebug
		{
			get
			{
				return debug;
			}
			set
			{
				debug = value;
			}
		}

		public string Host
		{
			get
			{
				return host;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
		}

		public string SessionId
		{
			get
			{
				return sessId;
			}
		}

		public int PollSpeed
		{
			get
			{
				return pollSpeed;
			}
			set
			{
				pollSpeed = ((value < 50 || value > 5000) ? 300 : value);
			}
		}

		public EventDispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public BBClient(BitSwarmClient bs)
		{
			debug = bs.Debug;
			log = bs.Log;
			if (dispatcher == null)
			{
				dispatcher = new EventDispatcher(this);
			}
		}

		public void Connect(string host, int port)
		{
			if (isConnected)
			{
				throw new Exception("BlueBox session is already connected");
			}
			this.host = host;
			this.port = port;
			bbUrl = "http://" + host + ":" + port + "/BlueBox/BlueBox.do";
			if (debug)
			{
				log.Debug("[ BB-Connect ]: " + bbUrl);
			}
			SendRequest("connect");
		}

		public void Send(ByteArray binData)
		{
			if (!isConnected)
			{
				throw new Exception("Can't send data, BlueBox connection is not active");
			}
			SendRequest("data", binData);
		}

		public void Close()
		{
			HandleConnectionLost(true);
		}

		internal void OnHttpResponse(bool error, string response)
		{
			if (error)
			{
				Hashtable hashtable = new Hashtable();
				hashtable["message"] = response;
				HandleConnectionLost(true);
				DispatchEvent(new BBEvent(BBEvent.IO_ERROR, hashtable));
				return;
			}
			try
			{
				if (debug)
				{
					log.Debug("[ BB-Receive ]: " + response.ToString());
				}
				string[] array = response.Split('|');
				if (array.Length < 2)
				{
					return;
				}
				string text = array[0];
				string text2 = array[1];
				switch (text)
				{
				case "connect":
					sessId = text2;
					isConnected = true;
					DispatchEvent(new BBEvent(BBEvent.CONNECT));
					Poll(null);
					break;
				case "poll":
				{
					ByteArray value = null;
					if (text2 != "null")
					{
						value = DecodeResponse(text2);
					}
					if (isConnected)
					{
						pollTimer = new Timer(Poll, null, pollSpeed, -1);
					}
					if (text2 != "null")
					{
						Hashtable hashtable3 = new Hashtable();
						hashtable3["data"] = value;
						DispatchEvent(new BBEvent(BBEvent.DATA, hashtable3));
					}
					break;
				}
				case "err01":
				{
					Hashtable hashtable2 = new Hashtable();
					hashtable2["message"] = "Invalid http session !";
					HandleConnectionLost(false);
					DispatchEvent(new BBEvent(BBEvent.IO_ERROR, hashtable2));
					break;
				}
				}
			}
			catch (Exception ex)
			{
				Hashtable hashtable4 = new Hashtable();
				hashtable4["message"] = ex.Message + " " + ex.StackTrace;
				HandleConnectionLost(false);
				DispatchEvent(new BBEvent(BBEvent.IO_ERROR, hashtable4));
			}
		}

		private void Poll(object state)
		{
			if (isConnected)
			{
				SendRequest("poll");
			}
		}

		private void SendRequest(string cmd)
		{
			SendRequest(cmd, null);
		}

		private void SendRequest(string cmd, object data)
		{
			string text = EncodeRequest(cmd, data);
			string encodedData = Uri.EscapeDataString(text);
			if (debug)
			{
				log.Debug("[ BB-Send ]: " + text);
			}
			SFSWebClient webClient = GetWebClient();
			webClient.UploadValuesAsync(new Uri(bbUrl), "sfsHttp", encodedData);
		}

		private SFSWebClient GetWebClient()
		{
			SFSWebClient sFSWebClient = new SFSWebClient();
			sFSWebClient.OnHttpResponse = (HttpResponseDelegate)Delegate.Combine(sFSWebClient.OnHttpResponse, new HttpResponseDelegate(OnHttpResponse));
			return sFSWebClient;
		}

		private void HandleConnectionLost(bool fireEvent)
		{
			if (isConnected)
			{
				isConnected = false;
				sessId = null;
				pollTimer.Dispose();
				if (fireEvent)
				{
					DispatchEvent(new BBEvent(BBEvent.DISCONNECT));
				}
			}
		}

		private string EncodeRequest(string cmd)
		{
			return EncodeRequest(cmd, null);
		}

		private string EncodeRequest(string cmd, object data)
		{
			string text = "";
			string text2 = "";
			if (cmd == null)
			{
				cmd = "null";
			}
			if (data == null)
			{
				text2 = "null";
			}
			else if (data is ByteArray)
			{
				text2 = Convert.ToBase64String(((ByteArray)data).Bytes);
			}
			return ((sessId != null) ? sessId : "null") + Convert.ToString('|') + cmd + Convert.ToString('|') + text2;
		}

		private ByteArray DecodeResponse(string rawData)
		{
			return new ByteArray(Convert.FromBase64String(rawData));
		}

		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}

		private void DispatchEvent(BaseEvent evt)
		{
			dispatcher.DispatchEvent(evt);
		}
	}
}
