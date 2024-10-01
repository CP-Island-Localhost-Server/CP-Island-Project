using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class ECDHCBasicAgreement : IBasicAgreement
	{
		private ECPrivateKeyParameters key;

		public virtual void Init(ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			key = (ECPrivateKeyParameters)parameters;
		}

		public virtual int GetFieldSize()
		{
			return (key.Parameters.Curve.FieldSize + 7) / 8;
		}

		public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
		{
			ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)pubKey;
			ECDomainParameters parameters = eCPublicKeyParameters.Parameters;
			BigInteger b = parameters.H.Multiply(key.D).Mod(parameters.N);
			ECPoint eCPoint = eCPublicKeyParameters.Q.Multiply(b).Normalize();
			if (eCPoint.IsInfinity)
			{
				throw new InvalidOperationException("Infinity is not a valid agreement value for ECDHC");
			}
			return eCPoint.AffineXCoord.ToBigInteger();
		}
	}
}
