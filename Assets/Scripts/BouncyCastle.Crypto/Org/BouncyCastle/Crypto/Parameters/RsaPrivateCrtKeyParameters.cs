using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class RsaPrivateCrtKeyParameters : RsaKeyParameters
	{
		private readonly BigInteger e;

		private readonly BigInteger p;

		private readonly BigInteger q;

		private readonly BigInteger dP;

		private readonly BigInteger dQ;

		private readonly BigInteger qInv;

		public BigInteger PublicExponent
		{
			get
			{
				return e;
			}
		}

		public BigInteger P
		{
			get
			{
				return p;
			}
		}

		public BigInteger Q
		{
			get
			{
				return q;
			}
		}

		public BigInteger DP
		{
			get
			{
				return dP;
			}
		}

		public BigInteger DQ
		{
			get
			{
				return dQ;
			}
		}

		public BigInteger QInv
		{
			get
			{
				return qInv;
			}
		}

		public RsaPrivateCrtKeyParameters(BigInteger modulus, BigInteger publicExponent, BigInteger privateExponent, BigInteger p, BigInteger q, BigInteger dP, BigInteger dQ, BigInteger qInv)
			: base(true, modulus, privateExponent)
		{
			ValidateValue(publicExponent, "publicExponent", "exponent");
			ValidateValue(p, "p", "P value");
			ValidateValue(q, "q", "Q value");
			ValidateValue(dP, "dP", "DP value");
			ValidateValue(dQ, "dQ", "DQ value");
			ValidateValue(qInv, "qInv", "InverseQ value");
			e = publicExponent;
			this.p = p;
			this.q = q;
			this.dP = dP;
			this.dQ = dQ;
			this.qInv = qInv;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = obj as RsaPrivateCrtKeyParameters;
			if (rsaPrivateCrtKeyParameters == null)
			{
				return false;
			}
			if (rsaPrivateCrtKeyParameters.DP.Equals(dP) && rsaPrivateCrtKeyParameters.DQ.Equals(dQ) && rsaPrivateCrtKeyParameters.Exponent.Equals(base.Exponent) && rsaPrivateCrtKeyParameters.Modulus.Equals(base.Modulus) && rsaPrivateCrtKeyParameters.P.Equals(p) && rsaPrivateCrtKeyParameters.Q.Equals(q) && rsaPrivateCrtKeyParameters.PublicExponent.Equals(e))
			{
				return rsaPrivateCrtKeyParameters.QInv.Equals(qInv);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return DP.GetHashCode() ^ DQ.GetHashCode() ^ base.Exponent.GetHashCode() ^ base.Modulus.GetHashCode() ^ P.GetHashCode() ^ Q.GetHashCode() ^ PublicExponent.GetHashCode() ^ QInv.GetHashCode();
		}

		private static void ValidateValue(BigInteger x, string name, string desc)
		{
			if (x == null)
			{
				throw new ArgumentNullException(name);
			}
			if (x.SignValue <= 0)
			{
				throw new ArgumentException("Not a valid RSA " + desc, name);
			}
		}
	}
}
