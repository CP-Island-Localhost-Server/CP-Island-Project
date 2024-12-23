using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public sealed class Srp6GroupParameters
	{
		private readonly BigInteger n;

		private readonly BigInteger g;

		public BigInteger G
		{
			get
			{
				return g;
			}
		}

		public BigInteger N
		{
			get
			{
				return n;
			}
		}

		public Srp6GroupParameters(BigInteger N, BigInteger g)
		{
			n = N;
			this.g = g;
		}
	}
}
