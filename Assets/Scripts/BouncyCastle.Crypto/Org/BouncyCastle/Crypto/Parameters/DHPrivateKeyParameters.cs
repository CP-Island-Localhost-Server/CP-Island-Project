using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DHPrivateKeyParameters : DHKeyParameters
	{
		private readonly BigInteger x;

		public BigInteger X
		{
			get
			{
				return x;
			}
		}

		public DHPrivateKeyParameters(BigInteger x, DHParameters parameters)
			: base(true, parameters)
		{
			this.x = x;
		}

		public DHPrivateKeyParameters(BigInteger x, DHParameters parameters, DerObjectIdentifier algorithmOid)
			: base(true, parameters, algorithmOid)
		{
			this.x = x;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHPrivateKeyParameters dHPrivateKeyParameters = obj as DHPrivateKeyParameters;
			if (dHPrivateKeyParameters == null)
			{
				return false;
			}
			return Equals(dHPrivateKeyParameters);
		}

		protected bool Equals(DHPrivateKeyParameters other)
		{
			if (x.Equals(other.x))
			{
				return Equals((DHKeyParameters)other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ base.GetHashCode();
		}
	}
}
