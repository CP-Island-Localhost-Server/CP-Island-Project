namespace Org.BouncyCastle.Asn1.X509
{
	public class CrlReason : DerEnumerated
	{
		public const int Unspecified = 0;

		public const int KeyCompromise = 1;

		public const int CACompromise = 2;

		public const int AffiliationChanged = 3;

		public const int Superseded = 4;

		public const int CessationOfOperation = 5;

		public const int CertificateHold = 6;

		public const int RemoveFromCrl = 8;

		public const int PrivilegeWithdrawn = 9;

		public const int AACompromise = 10;

		private static readonly string[] ReasonString = new string[11]
		{
			"Unspecified", "KeyCompromise", "CACompromise", "AffiliationChanged", "Superseded", "CessationOfOperation", "CertificateHold", "Unknown", "RemoveFromCrl", "PrivilegeWithdrawn",
			"AACompromise"
		};

		public CrlReason(int reason)
			: base(reason)
		{
		}

		public CrlReason(DerEnumerated reason)
			: base(reason.Value.IntValue)
		{
		}

		public override string ToString()
		{
			int intValue = base.Value.IntValue;
			string text = ((intValue < 0 || intValue > 10) ? "Invalid" : ReasonString[intValue]);
			return "CrlReason: " + text;
		}
	}
}
