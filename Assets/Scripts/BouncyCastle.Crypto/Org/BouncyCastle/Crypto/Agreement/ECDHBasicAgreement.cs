using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class ECDHBasicAgreement : IBasicAgreement
	{
		protected internal ECPrivateKeyParameters privKey;

		public virtual void Init(ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			privKey = (ECPrivateKeyParameters)parameters;
		}

		public virtual int GetFieldSize()
		{
			return (privKey.Parameters.Curve.FieldSize + 7) / 8;
		}

		public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
		{
			ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)pubKey;
			ECPoint eCPoint = eCPublicKeyParameters.Q.Multiply(privKey.D).Normalize();
			if (eCPoint.IsInfinity)
			{
				throw new InvalidOperationException("Infinity is not a valid agreement value for ECDH");
			}
			return eCPoint.AffineXCoord.ToBigInteger();
		}
	}
}
