using System.IO;

namespace Org.BouncyCastle.Crypto.Operators
{
	internal class VerifierCalculator : IStreamCalculator
	{
		private readonly ISigner sig;

		private readonly Stream stream;

		public Stream Stream
		{
			get
			{
				return stream;
			}
		}

		internal VerifierCalculator(ISigner sig)
		{
			this.sig = sig;
			stream = new SignerBucket(sig);
		}

		public object GetResult()
		{
			return new VerifierResult(sig);
		}
	}
}
