using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class SubsequentMessage : DerInteger
	{
		public static readonly SubsequentMessage encrCert = new SubsequentMessage(0);

		public static readonly SubsequentMessage challengeResp = new SubsequentMessage(1);

		private SubsequentMessage(int value)
			: base(value)
		{
		}

		public static SubsequentMessage ValueOf(int value)
		{
			switch (value)
			{
			case 0:
				return encrCert;
			case 1:
				return challengeResp;
			default:
				throw new ArgumentException("unknown value: " + value, "value");
			}
		}
	}
}
