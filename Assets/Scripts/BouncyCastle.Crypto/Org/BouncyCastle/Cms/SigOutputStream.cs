using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Cms
{
	internal class SigOutputStream : BaseOutputStream
	{
		private readonly ISigner sig;

		internal SigOutputStream(ISigner sig)
		{
			this.sig = sig;
		}

		public override void WriteByte(byte b)
		{
			try
			{
				sig.Update(b);
			}
			catch (SignatureException ex)
			{
				throw new CmsStreamException("signature problem: " + ex);
			}
		}

		public override void Write(byte[] b, int off, int len)
		{
			try
			{
				sig.BlockUpdate(b, off, len);
			}
			catch (SignatureException ex)
			{
				throw new CmsStreamException("signature problem: " + ex);
			}
		}
	}
}
