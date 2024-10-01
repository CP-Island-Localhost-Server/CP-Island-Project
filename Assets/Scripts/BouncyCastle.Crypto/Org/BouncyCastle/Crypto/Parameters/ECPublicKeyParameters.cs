using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ECPublicKeyParameters : ECKeyParameters
	{
		private readonly ECPoint q;

		public ECPoint Q
		{
			get
			{
				return q;
			}
		}

		public ECPublicKeyParameters(ECPoint q, ECDomainParameters parameters)
			: this("EC", q, parameters)
		{
		}

		[Obsolete("Use version with explicit 'algorithm' parameter")]
		public ECPublicKeyParameters(ECPoint q, DerObjectIdentifier publicKeyParamSet)
			: base("ECGOST3410", false, publicKeyParamSet)
		{
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			this.q = q.Normalize();
		}

		public ECPublicKeyParameters(string algorithm, ECPoint q, ECDomainParameters parameters)
			: base(algorithm, false, parameters)
		{
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			this.q = q.Normalize();
		}

		public ECPublicKeyParameters(string algorithm, ECPoint q, DerObjectIdentifier publicKeyParamSet)
			: base(algorithm, false, publicKeyParamSet)
		{
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			this.q = q.Normalize();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ECPublicKeyParameters eCPublicKeyParameters = obj as ECPublicKeyParameters;
			if (eCPublicKeyParameters == null)
			{
				return false;
			}
			return Equals(eCPublicKeyParameters);
		}

		protected bool Equals(ECPublicKeyParameters other)
		{
			if (q.Equals(other.q))
			{
				return Equals((ECKeyParameters)other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return q.GetHashCode() ^ base.GetHashCode();
		}
	}
}
