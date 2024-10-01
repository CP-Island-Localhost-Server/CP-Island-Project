using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class NaccacheSternKeyParameters : AsymmetricKeyParameter
	{
		private readonly BigInteger g;

		private readonly BigInteger n;

		private readonly int lowerSigmaBound;

		public BigInteger G
		{
			get
			{
				return g;
			}
		}

		public int LowerSigmaBound
		{
			get
			{
				return lowerSigmaBound;
			}
		}

		public BigInteger Modulus
		{
			get
			{
				return n;
			}
		}

		public NaccacheSternKeyParameters(bool privateKey, BigInteger g, BigInteger n, int lowerSigmaBound)
			: base(privateKey)
		{
			this.g = g;
			this.n = n;
			this.lowerSigmaBound = lowerSigmaBound;
		}
	}
}
