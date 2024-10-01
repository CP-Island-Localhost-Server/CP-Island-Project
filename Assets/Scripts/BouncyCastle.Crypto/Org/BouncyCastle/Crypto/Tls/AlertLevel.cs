namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class AlertLevel
	{
		public const byte warning = 1;

		public const byte fatal = 2;

		public static string GetName(byte alertDescription)
		{
			switch (alertDescription)
			{
			case 1:
				return "warning";
			case 2:
				return "fatal";
			default:
				return "UNKNOWN";
			}
		}

		public static string GetText(byte alertDescription)
		{
			return GetName(alertDescription) + "(" + alertDescription + ")";
		}
	}
}
