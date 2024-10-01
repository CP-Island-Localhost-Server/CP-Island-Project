using System.Collections;
using Sfs2X.Core;

namespace Sfs2X.Bitswarm
{
	public class BitSwarmEvent : BaseEvent
	{
		public static readonly string CONNECT = "connect";

		public static readonly string DISCONNECT = "disconnect";

		public static readonly string RECONNECTION_TRY = "reconnectionTry";

		public static readonly string IO_ERROR = "ioError";

		public static readonly string SECURITY_ERROR = "securityError";

		public static readonly string DATA_ERROR = "dataError";

		public BitSwarmEvent(string type)
			: base(type, null)
		{
		}

		public BitSwarmEvent(string type, Hashtable arguments)
			: base(type, arguments)
		{
		}
	}
}
