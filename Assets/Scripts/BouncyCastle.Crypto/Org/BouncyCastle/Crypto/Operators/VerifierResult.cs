using System;

namespace Org.BouncyCastle.Crypto.Operators
{
	internal class VerifierResult : IVerifier
	{
		private readonly ISigner sig;

		internal VerifierResult(ISigner sig)
		{
			this.sig = sig;
		}

		public bool IsVerified(byte[] signature)
		{
			return sig.VerifySignature(signature);
		}

		public bool IsVerified(byte[] signature, int off, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(signature, 0, array, off, array.Length);
			return sig.VerifySignature(signature);
		}
	}
}
