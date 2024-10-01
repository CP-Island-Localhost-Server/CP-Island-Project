using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DsaPublicKeyParameters : DsaKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y
		{
			get
			{
				return y;
			}
		}

		public DsaPublicKeyParameters(BigInteger y, DsaParameters parameters)
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
			DsaPublicKeyParameters dsaPublicKeyParameters = obj as DsaPublicKeyParameters;
			if (dsaPublicKeyParameters == null)
			{
				return false;
			}
			return Equals(dsaPublicKeyParameters);
		}

		protected bool Equals(DsaPublicKeyParameters other)
		{
			if (y.Equals(other.y))
			{
				return Equals((DsaKeyParameters)other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
