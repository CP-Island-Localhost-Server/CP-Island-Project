using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Sfs2X.Bitswarm;
using Sfs2X.Core;
using Sfs2X.Core.Sockets;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Util;
using Sfs2X.Util.LagMonitor;

namespace Sfs2X
{
	public class SmartFox : IDispatchable
	{
		private const int DEFAULT_HTTP_PORT = 8080;

		private const char CLIENT_TYPE_SEPARATOR = ':';

		private int majVersion = 1;

		private int minVersion = 7;

		private int subVersion = 2;

		private ISocketClient socketClient;

		private string clientDetails = "Unity";

		private ILagMonitor lagMonitor;

		private UseWebSocket? useWebSocket = null;

		private bool useBlueBox = true;

		private bool isJoining = false;

		private User mySelf;

		private string sessionToken;

		private Room lastJoinedRoom;

		private Logger log;

		private bool inited = false;

		private bool debug = false;

		private bool threadSafeMode = true;

		private bool isConnecting = false;

		private IUserManager userManager;

		private IRoomManager roomManager;

		private IBuddyManager buddyManager;

		private ConfigData config;

		private string currentZone;

		private bool autoConnectOnConfig = false;

		private string lastHost;

		private EventDispatcher dispatcher;

		private object eventsLocker = new object();

		private Queue<BaseEvent> eventsQueue = new Queue<BaseEvent>();

		public ISocketClient SocketClient
		{
			get
			{
				return socketClient;
			}
		}

		public Logger Log
		{
			get
			{
				return log;
			}
		}

		public bool IsConnecting
		{
			get
			{
				return isConnecting;
			}
		}

		public ILagMonitor LagMonitor
		{
			get
			{
				return lagMonitor;
			}
		}

		public bool IsConnected
		{
			get
			{
				bool result = false;
				if (socketClient != null)
				{
					result = socketClient.Connected;
				}
				return result;
			}
		}

		public string Version
		{
			get
			{
				return "" + majVersion + "." + minVersion + "." + subVersion;
			}
		}

		public string HttpUploadURI
		{
			get
			{
				if (config == null || mySelf == null)
				{
					return null;
				}
				return "http://" + config.Host + ":" + config.HttpPort + "/BlueBox/SFS2XFileUpload?sessHashId=" + sessionToken;
			}
		}

		public ConfigData Config
		{
			get
			{
				return config;
			}
		}

		public bool UseBlueBox
		{
			get
			{
				return useBlueBox;
			}
			set
			{
				useBlueBox = value;
			}
		}

		public string ConnectionMode
		{
			get
			{
				return socketClient.ConnectionMode;
			}
		}

		public int CompressionThreshold
		{
			get
			{
				return socketClient.CompressionThreshold;
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return socketClient.MaxMessageSize;
			}
		}

		public bool Debug
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

		public string CurrentIp
		{
			get
			{
				return socketClient.ConnectionHost;
			}
		}

		public int CurrentPort
		{
			get
			{
				return socketClient.ConnectionPort;
			}
		}

		public string CurrentZone
		{
			get
			{
				return currentZone;
			}
		}

		public User MySelf
		{
			get
			{
				return mySelf;
			}
			set
			{
				mySelf = value;
			}
		}

		public Logger Logger
		{
			get
			{
				return log;
			}
		}

		public Room LastJoinedRoom
		{
			get
			{
				return lastJoinedRoom;
			}
			set
			{
				lastJoinedRoom = value;
			}
		}

		public List<Room> JoinedRooms
		{
			get
			{
				return roomManager.GetJoinedRooms();
			}
		}

		public List<Room> RoomList
		{
			get
			{
				return roomManager.GetRoomList();
			}
		}

		public IRoomManager RoomManager
		{
			get
			{
				return roomManager;
			}
		}

		public IUserManager UserManager
		{
			get
			{
				return userManager;
			}
		}

		public IBuddyManager BuddyManager
		{
			get
			{
				return buddyManager;
			}
		}

		public bool UdpAvailable
		{
			get
			{
				return socketClient.UdpManager != null;
			}
		}

		public bool UdpInited
		{
			get
			{
				if (socketClient.UdpManager != null)
				{
					return socketClient.UdpManager.Inited;
				}
				return false;
			}
		}

		public bool IsJoining
		{
			get
			{
				return isJoining;
			}
			set
			{
				isJoining = value;
			}
		}

		public string SessionToken
		{
			get
			{
				return sessionToken;
			}
		}

		public EventDispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public bool ThreadSafeMode
		{
			get
			{
				return threadSafeMode;
			}
			set
			{
				threadSafeMode = value;
			}
		}

		public SmartFox()
		{
			Initialize(false);
		}

		public SmartFox(bool debug)
		{
			Initialize(debug);
		}

		public SmartFox(UseWebSocket useWebSocket)
		{
			this.useWebSocket = useWebSocket;
			Initialize(false);
		}

		public SmartFox(UseWebSocket useWebSocket, bool debug)
		{
			this.useWebSocket = useWebSocket;
			Initialize(debug);
		}

		private void Initialize(bool debug)
		{
			if (!inited)
			{
				log = new Logger(this);
				this.debug = debug;
				if (dispatcher == null)
				{
					dispatcher = new EventDispatcher(this);
				}
				UseWebSocket? useWebSocket = this.useWebSocket;
				if (useWebSocket.HasValue)
				{
					socketClient = new WebSocketClient(this, this.useWebSocket == UseWebSocket.WSS);
					socketClient.IoHandler = new WSIOHandler(socketClient);
				}
				else
				{
					socketClient = new BitSwarmClient(this);
					socketClient.IoHandler = new SFSIOHandler(socketClient);
				}
				socketClient.Init();
				socketClient.Dispatcher.AddEventListener(BitSwarmEvent.CONNECT, OnSocketConnect);
				socketClient.Dispatcher.AddEventListener(BitSwarmEvent.DISCONNECT, OnSocketClose);
				socketClient.Dispatcher.AddEventListener(BitSwarmEvent.RECONNECTION_TRY, OnSocketReconnectionTry);
				socketClient.Dispatcher.AddEventListener(BitSwarmEvent.IO_ERROR, OnSocketIOError);
				socketClient.Dispatcher.AddEventListener(BitSwarmEvent.SECURITY_ERROR, OnSocketSecurityError);
				socketClient.Dispatcher.AddEventListener(BitSwarmEvent.DATA_ERROR, OnSocketDataError);
				inited = true;
				Reset();
			}
		}

		private void Reset()
		{
			userManager = new SFSGlobalUserManager(this);
			roomManager = new SFSRoomManager(this);
			buddyManager = new SFSBuddyManager(this);
			if (lagMonitor != null)
			{
				lagMonitor.Destroy();
			}
			isJoining = false;
			currentZone = null;
			lastJoinedRoom = null;
			sessionToken = null;
			mySelf = null;
		}

		public void SetClientDetails(string platformId, string version)
		{
			if (IsConnected)
			{
				log.Warn("SetClientDetails must be called before the connection is started");
			}
			else
			{
				clientDetails = ((platformId == null) ? "" : platformId.Replace(':', ' '));
				clientDetails += ':';
				clientDetails += ((version == null) ? "" : version.Replace(':', ' '));
			}
		}

		public void EnableLagMonitor(bool enabled, int interval, int queueSize)
		{
			if (mySelf == null)
			{
				log.Warn("Lag Monitoring requires that you are logged in a Zone!");
			}
			else if (enabled)
			{
				lagMonitor = new DefaultLagMonitor(this, interval, queueSize);
				lagMonitor.Start();
			}
			else
			{
				lagMonitor.Stop();
			}
		}

		public void EnableLagMonitor(bool enabled)
		{
			EnableLagMonitor(enabled, 4, 10);
		}

		public void EnableLagMonitor(bool enabled, int interval)
		{
			EnableLagMonitor(enabled, interval, 10);
		}

		public ISocketClient GetSocketEngine()
		{
			return socketClient;
		}

		public Room GetRoomById(int id)
		{
			return roomManager.GetRoomById(id);
		}

		public Room GetRoomByName(string name)
		{
			return roomManager.GetRoomByName(name);
		}

		public List<Room> GetRoomListFromGroup(string groupId)
		{
			return roomManager.GetRoomListFromGroup(groupId);
		}

		public void KillConnection()
		{
			socketClient.KillConnection();
		}

		public void Connect(string host, int port)
		{
			if (IsConnected)
			{
				log.Warn("Already connected");
				return;
			}
			if (isConnecting)
			{
				log.Warn("A connection attempt is already in progress");
				return;
			}
			if (config == null)
			{
				config = new ConfigData();
				config.UseBlueBox = UseBlueBox;
				config.Debug = Debug;
			}
			if (host == null)
			{
				host = config.Host;
			}
			if (port == -1)
			{
				port = config.Port;
			}
			if (host == null || host.Length == 0)
			{
				throw new ArgumentException("Invalid connection host name / IP address");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentException("Invalid connection port");
			}
			lastHost = host;
			isConnecting = true;
			socketClient.Connect(host, port);
		}

		public void Connect()
		{
			Connect(null, -1);
		}

		public void Connect(string host)
		{
			Connect(host, -1);
		}

		public void Connect(ConfigData cfg)
		{
			ValidateConfig(cfg);
			Connect(cfg.Host, cfg.Port);
		}

		public void Disconnect()
		{
			if (IsConnected)
			{
				if (socketClient.ReconnectionSeconds > 0)
				{
					Send(new ManualDisconnectionRequest());
					int millisecondsTimeout = 100;
					Thread.Sleep(millisecondsTimeout);
				}
				HandleClientDisconnection(ClientDisconnectionReason.MANUAL);
			}
			else
			{
				log.Info("You are not connected");
			}
		}

		public void InitUDP(string udpHost, int udpPort)
		{
			if (!IsConnected)
			{
				Logger.Warn("Cannot initialize UDP protocol until the client is connected to SFS2X");
				return;
			}
			if (MySelf == null)
			{
				Logger.Warn("Cannot initialize UDP protocol until the user is logged-in");
				return;
			}
			if (socketClient.UdpManager == null || !socketClient.UdpManager.Inited)
			{
				UseWebSocket? useWebSocket = this.useWebSocket;
				if (useWebSocket.HasValue)
				{
					Logger.Warn("UDP not supported in WebSocket mode");
					return;
				}
				IUDPManager udpManager = new UDPManager(this);
				socketClient.UdpManager = udpManager;
			}
			if (socketClient.UdpManager == null)
			{
				return;
			}
			if (config != null)
			{
				if (udpHost == null)
				{
					udpHost = config.UdpHost;
				}
				if (udpPort == -1)
				{
					udpPort = config.UdpPort;
				}
			}
			if (udpHost == null || udpHost.Length == 0)
			{
				throw new ArgumentException("Invalid UDP host/address");
			}
			if (udpPort < 0 || udpPort > 65535)
			{
				throw new ArgumentException("Invalid UDP port range");
			}
			try
			{
				socketClient.UdpManager.Initialize(udpHost, udpPort);
			}
			catch (Exception ex)
			{
				log.Error("Exception initializing UDP: " + ex.Message);
			}
		}

		public void InitUDP()
		{
			InitUDP(null, -1);
		}

		public void InitUDP(string udpHost)
		{
			InitUDP(udpHost, -1);
		}

		public IEnumerator InitCrypto()
		{
			UseWebSocket? useWebSocket = this.useWebSocket;
			if (useWebSocket.HasValue)
			{
				Logger.Warn("InitCrypto method not supported in WebSocket mode; use WSS protocol instead");
				return null;
			}
			CryptoInitializer cryptoInitializer = new CryptoInitializer(this);
			return cryptoInitializer.Run();
		}

		public int GetReconnectionSeconds()
		{
			return socketClient.ReconnectionSeconds;
		}

		public void SetReconnectionSeconds(int seconds)
		{
			socketClient.ReconnectionSeconds = seconds;
		}

		public void Send(IRequest request)
		{
			if (!IsConnected)
			{
				log.Warn("You are not connected. Request cannot be sent: " + request);
				return;
			}
			try
			{
				if (request is JoinRoomRequest)
				{
					if (isJoining)
					{
						return;
					}
					isJoining = true;
				}
				request.Validate(this);
				request.Execute(this);
				socketClient.Send(request.Message);
			}
			catch (SFSValidationError sFSValidationError)
			{
				string text = sFSValidationError.Message;
				foreach (string error in sFSValidationError.Errors)
				{
					text = text + "\t" + error + "\n";
				}
				log.Warn(text);
			}
			catch (SFSCodecError sFSCodecError)
			{
				log.Warn(sFSCodecError.Message);
			}
		}

		public void LoadConfig(string filePath, bool connectOnSuccess)
		{
			ConfigLoader configLoader = new ConfigLoader(this);
			configLoader.Dispatcher.AddEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccess);
			configLoader.Dispatcher.AddEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailure);
			autoConnectOnConfig = connectOnSuccess;
			configLoader.LoadConfig(filePath);
		}

		public void LoadConfig(string filePath)
		{
			LoadConfig(filePath, true);
		}

		public void LoadConfig(bool connectOnSuccess)
		{
			LoadConfig("sfs-config.xml", connectOnSuccess);
		}

		public void LoadConfig()
		{
			LoadConfig("sfs-config.xml", true);
		}

		public void AddLogListener(LogLevel logLevel, EventListenerDelegate eventListener)
		{
			AddEventListener(LoggerEvent.LogEventType(logLevel), eventListener);
			log.EnableEventDispatching = true;
		}

		public void RemoveLogListener(LogLevel logLevel, EventListenerDelegate eventListener)
		{
			RemoveEventListener(LoggerEvent.LogEventType(logLevel), eventListener);
		}

		public void AddJoinedRoom(Room room)
		{
			if (!roomManager.ContainsRoom(room.Id))
			{
				roomManager.AddRoom(room);
				lastJoinedRoom = room;
				return;
			}
			throw new SFSError("Unexpected: joined room already exists for this User: " + mySelf.Name + ", Room: " + room);
		}

		public void RemoveJoinedRoom(Room room)
		{
			roomManager.RemoveRoom(room);
			if (JoinedRooms.Count > 0)
			{
				lastJoinedRoom = JoinedRooms[JoinedRooms.Count - 1];
			}
		}

		private void OnSocketConnect(BaseEvent e)
		{
			BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
			if ((bool)bitSwarmEvent.Params["success"])
			{
				SendHandshakeRequest((bool)bitSwarmEvent.Params["isReconnection"]);
				return;
			}
			log.Warn("Connection attempt failed");
			HandleConnectionProblem(bitSwarmEvent);
		}

		private void OnSocketClose(BaseEvent e)
		{
			BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
			Reset();
			Hashtable hashtable = new Hashtable();
			hashtable["reason"] = bitSwarmEvent.Params["reason"];
			DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_LOST, hashtable));
		}

		private void OnSocketReconnectionTry(BaseEvent e)
		{
			DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_RETRY));
		}

		private void OnSocketDataError(BaseEvent e)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["errorMessage"] = e.Params["message"];
			DispatchEvent(new SFSEvent(SFSEvent.SOCKET_ERROR, hashtable));
		}

		private void OnSocketIOError(BaseEvent e)
		{
			BitSwarmEvent e2 = e as BitSwarmEvent;
			if (isConnecting)
			{
				HandleConnectionProblem(e2);
			}
		}

		private void OnSocketSecurityError(BaseEvent e)
		{
			BitSwarmEvent e2 = e as BitSwarmEvent;
			if (isConnecting)
			{
				HandleConnectionProblem(e2);
			}
		}

		private void OnConfigLoadSuccess(BaseEvent e)
		{
			SFSEvent sFSEvent = e as SFSEvent;
			ConfigLoader configLoader = sFSEvent.Target as ConfigLoader;
			ConfigData configData = sFSEvent.Params["cfg"] as ConfigData;
			configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccess);
			configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailure);
			ValidateConfig(configData);
			Hashtable hashtable = new Hashtable();
			hashtable["config"] = configData;
			BaseEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_SUCCESS, hashtable);
			DispatchEvent(evt);
			if (autoConnectOnConfig)
			{
				Connect(config.Host, config.Port);
			}
		}

		private void OnConfigLoadFailure(BaseEvent e)
		{
			SFSEvent sFSEvent = e as SFSEvent;
			log.Error("Failed to load config: " + (string)sFSEvent.Params["message"]);
			ConfigLoader configLoader = sFSEvent.Target as ConfigLoader;
			configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccess);
			configLoader.Dispatcher.RemoveEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailure);
			BaseEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_FAILURE);
			DispatchEvent(evt);
		}

		private void ValidateConfig(ConfigData cfgData)
		{
			if (cfgData.Host == null || cfgData.Host.Length == 0)
			{
				throw new ArgumentException("Invalid host name / IP address in configuration data");
			}
			if (cfgData.Port < 0 || cfgData.Port > 65535)
			{
				throw new ArgumentException("Invalid TCP port in configuration data");
			}
			if (cfgData.Zone == null || cfgData.Zone.Length == 0)
			{
				throw new ArgumentException("Invalid Zone name in configuration data");
			}
			config = cfgData;
			debug = cfgData.Debug;
			useBlueBox = cfgData.UseBlueBox;
		}

		public void HandleHandShake(BaseEvent evt)
		{
			ISFSObject iSFSObject = evt.Params["message"] as ISFSObject;
			if (iSFSObject.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				sessionToken = iSFSObject.GetUtfString(HandshakeRequest.KEY_SESSION_TOKEN);
				socketClient.CompressionThreshold = iSFSObject.GetInt(HandshakeRequest.KEY_COMPRESSION_THRESHOLD);
				socketClient.MaxMessageSize = iSFSObject.GetInt(HandshakeRequest.KEY_MAX_MESSAGE_SIZE);
				if (debug)
				{
					log.Debug(string.Format("Handshake response: tk => {0}, ct => {1}", sessionToken, socketClient.CompressionThreshold));
				}
				if (socketClient.IsReconnecting)
				{
					socketClient.IsReconnecting = false;
					DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_RESUME));
					return;
				}
				isConnecting = false;
				Hashtable hashtable = new Hashtable();
				hashtable["success"] = true;
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable));
			}
			else
			{
				short @short = iSFSObject.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, log, iSFSObject.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				Hashtable hashtable2 = new Hashtable();
				hashtable2["success"] = false;
				hashtable2["errorMessage"] = errorMessage;
				hashtable2["errorCode"] = @short;
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable2));
			}
		}

		public void HandleLogin(BaseEvent evt)
		{
			currentZone = evt.Params["zone"] as string;
		}

		public void HandleClientDisconnection(string reason)
		{
			socketClient.ReconnectionSeconds = 0;
			socketClient.Disconnect(reason);
			Reset();
			if (reason != null)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("reason", reason);
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_LOST, hashtable));
			}
		}

		public void HandleLogout()
		{
			if (lagMonitor != null && lagMonitor.IsRunning)
			{
				lagMonitor.Stop();
			}
			userManager = new SFSGlobalUserManager(this);
			roomManager = new SFSRoomManager(this);
			isJoining = false;
			lastJoinedRoom = null;
			currentZone = null;
			mySelf = null;
		}

		private void HandleConnectionProblem(BaseEvent e)
		{
			if (socketClient.ConnectionMode == ConnectionModes.SOCKET && useBlueBox)
			{
				socketClient.ForceBlueBox(true);
				int port = ((config == null) ? 8080 : config.HttpPort);
				socketClient.Connect(lastHost, port);
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_ATTEMPT_HTTP, new Hashtable()));
				return;
			}
			if (socketClient.ConnectionMode != ConnectionModes.WEBSOCKET && socketClient.ConnectionMode != ConnectionModes.WEBSOCKET_SECURE)
			{
				socketClient.ForceBlueBox(false);
			}
			BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
			Hashtable hashtable = new Hashtable();
			hashtable["success"] = false;
			hashtable["errorMessage"] = bitSwarmEvent.Params["message"];
			DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable));
			isConnecting = false;
			socketClient.Destroy();
		}

		public void HandleReconnectionFailure()
		{
			SetReconnectionSeconds(0);
			socketClient.StopReconnection();
		}

		private void SendHandshakeRequest(bool isReconnection)
		{
			IRequest request = new HandshakeRequest(Version, (!isReconnection) ? null : sessionToken, clientDetails);
			Send(request);
		}

		internal void DispatchEvent(BaseEvent evt)
		{
			if (!threadSafeMode)
			{
				Dispatcher.DispatchEvent(evt);
			}
			else
			{
				EnqueueEvent(evt);
			}
		}

		private void EnqueueEvent(BaseEvent evt)
		{
			lock (eventsLocker)
			{
				eventsQueue.Enqueue(evt);
			}
		}

		public void ProcessEvents()
		{
			if (!threadSafeMode)
			{
				return;
			}
			UseWebSocket? useWebSocket = this.useWebSocket;
			if (useWebSocket.HasValue && socketClient != null)
			{
				WebSocketLayer webSocketLayer = socketClient.Socket as WebSocketLayer;
				if (webSocketLayer != null)
				{
					webSocketLayer.ProcessState();
				}
			}
			BaseEvent[] array;
			lock (eventsLocker)
			{
				array = eventsQueue.ToArray();
				eventsQueue.Clear();
			}
			for (int i = 0; i < array.Length; i++)
			{
				Dispatcher.DispatchEvent(array[i]);
			}
		}

		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}

		public void RemoveEventListener(string eventType, EventListenerDelegate listener)
		{
			dispatcher.RemoveEventListener(eventType, listener);
		}

		public void RemoveAllEventListeners()
		{
			dispatcher.RemoveAll();
		}
	}
}
