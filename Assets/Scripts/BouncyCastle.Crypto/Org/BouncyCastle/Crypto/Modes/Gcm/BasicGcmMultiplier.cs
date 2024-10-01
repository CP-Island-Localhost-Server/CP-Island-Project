namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	public class BasicGcmMultiplier : IGcmMultiplier
	{
		private uint[] H;

		public void Init(byte[] H)
		{
			this.H = GcmUtilities.AsUints(H);
		}

		public void MultiplyH(byte[] x)
		{
			uint[] x2 = GcmUtilities.AsUints(x);
			GcmUtilities.Multiply(x2, H);
			GcmUtilities.AsBytes(x2, x);
		}
	}
}
