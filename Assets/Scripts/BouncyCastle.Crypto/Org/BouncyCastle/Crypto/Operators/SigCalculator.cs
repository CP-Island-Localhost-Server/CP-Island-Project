using System.IO;

namespace Org.BouncyCastle.Crypto.Operators
{
	internal class SigCalculator : IStreamCalculator
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

		internal SigCalculator(ISigner sig)
		{
			this.sig = sig;
			stream = new SignerBucket(sig);
		}

		public object GetResult()
		{
			return new SigResult(sig);
		}
	}
}
