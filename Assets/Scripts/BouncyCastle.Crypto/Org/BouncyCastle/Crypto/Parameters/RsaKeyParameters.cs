using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class RsaKeyParameters : AsymmetricKeyParameter
	{
		private readonly BigInteger modulus;

		private readonly BigInteger exponent;

		public BigInteger Modulus
		{
			get
			{
				return modulus;
			}
		}

		public BigInteger Exponent
		{
			get
			{
				return exponent;
			}
		}

		public RsaKeyParameters(bool isPrivate, BigInteger modulus, BigInteger exponent)
			: base(isPrivate)
		{
			if (modulus == null)
			{
				throw new ArgumentNullException("modulus");
			}
			if (exponent == null)
			{
				throw new ArgumentNullException("exponent");
			}
			if (modulus.SignValue <= 0)
			{
				throw new ArgumentException("Not a valid RSA modulus", "modulus");
			}
			if (exponent.SignValue <= 0)
			{
				throw new ArgumentException("Not a valid RSA exponent", "exponent");
			}
			this.modulus = modulus;
			this.exponent = exponent;
		}

		public override bool Equals(object obj)
		{
			RsaKeyParameters rsaKeyParameters = obj as RsaKeyParameters;
			if (rsaKeyParameters == null)
			{
				return false;
			}
			if (rsaKeyParameters.IsPrivate == base.IsPrivate && rsaKeyParameters.Modulus.Equals(modulus))
			{
				return rsaKeyParameters.Exponent.Equals(exponent);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return modulus.GetHashCode() ^ exponent.GetHashCode() ^ base.IsPrivate.GetHashCode();
		}
	}
}
