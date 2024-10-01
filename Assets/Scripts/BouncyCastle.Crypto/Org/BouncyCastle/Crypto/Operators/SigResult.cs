using System;

namespace Org.BouncyCastle.Crypto.Operators
{
	internal class SigResult : IBlockResult
	{
		private readonly ISigner sig;

		internal SigResult(ISigner sig)
		{
			this.sig = sig;
		}

		public byte[] Collect()
		{
			return sig.GenerateSignature();
		}

		public int Collect(byte[] destination, int offset)
		{
			byte[] array = Collect();
			Array.Copy(array, 0, destination, offset, array.Length);
			return array.Length;
		}
	}
}
