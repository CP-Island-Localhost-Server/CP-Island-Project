namespace Sfs2X.Util
{
	public class ConfigData
	{
		private string host = "127.0.0.1";

		private int port = 9933;

		private string udpHost = "127.0.0.1";

		private int udpPort = 9933;

		private string zone;

		private bool debug = false;

		private int httpPort = 8080;

		private int httpsPort = 8443;

		private bool useBlueBox = true;

		private int blueBoxPollingRate = 750;

		public string Host
		{
			get
			{
				return host;
			}
			set
			{
				host = value;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

		public string UdpHost
		{
			get
			{
				return udpHost;
			}
			set
			{
				udpHost = value;
			}
		}

		public int UdpPort
		{
			get
			{
				return udpPort;
			}
			set
			{
				udpPort = value;
			}
		}

		public string Zone
		{
			get
			{
				return zone;
			}
			set
			{
				zone = value;
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

		public int HttpPort
		{
			get
			{
				return httpPort;
			}
			set
			{
				httpPort = value;
			}
		}

		public int HttpsPort
		{
			get
			{
				return httpsPort;
			}
			set
			{
				httpsPort = value;
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

		public int BlueBoxPollingRate
		{
			get
			{
				return blueBoxPollingRate;
			}
			set
			{
				blueBoxPollingRate = value;
			}
		}
	}
}
