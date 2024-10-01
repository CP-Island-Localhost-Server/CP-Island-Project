using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ElGamalPublicKeyParameters : ElGamalKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y
		{
			get
			{
				return y;
			}
		}

		public ElGamalPublicKeyParameters(BigInteger y, ElGamalParameters parameters)
			: base(false, parameters)
		{
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			this.y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalPublicKeyParameters elGamalPublicKeyParameters = obj as ElGamalPublicKeyParameters;
			if (elGamalPublicKeyParameters == null)
			{
				return false;
			}
			return Equals(elGamalPublicKeyParameters);
		}

		protected bool Equals(ElGamalPublicKeyParameters other)
		{
			if (y.Equals(other.y))
			{
				return Equals((ElGamalKeyParameters)other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
