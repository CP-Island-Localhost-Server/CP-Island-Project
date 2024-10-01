namespace Sfs2X.Util
{
	public static class ClientDisconnectionReason
	{
		public static readonly string IDLE = "idle";

		public static readonly string KICK = "kick";

		public static readonly string BAN = "ban";

		public static readonly string MANUAL = "manual";

		public static readonly string UNKNOWN = "unknown";

		private static string[] reasons = new string[3] { "idle", "kick", "ban" };

		public static string GetReason(int reasonId)
		{
			return reasons[reasonId];
		}
	}
}
