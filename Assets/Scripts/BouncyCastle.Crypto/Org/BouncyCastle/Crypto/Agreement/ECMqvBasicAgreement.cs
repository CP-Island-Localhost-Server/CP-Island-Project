using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class ECMqvBasicAgreement : IBasicAgreement
	{
		protected internal MqvPrivateParameters privParams;

		public virtual void Init(ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			privParams = (MqvPrivateParameters)parameters;
		}

		public virtual int GetFieldSize()
		{
			return (privParams.StaticPrivateKey.Parameters.Curve.FieldSize + 7) / 8;
		}

		public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
		{
			MqvPublicParameters mqvPublicParameters = (MqvPublicParameters)pubKey;
			ECPrivateKeyParameters staticPrivateKey = privParams.StaticPrivateKey;
			ECPoint eCPoint = CalculateMqvAgreement(staticPrivateKey.Parameters, staticPrivateKey, privParams.EphemeralPrivateKey, privParams.EphemeralPublicKey, mqvPublicParameters.StaticPublicKey, mqvPublicParameters.EphemeralPublicKey).Normalize();
			if (eCPoint.IsInfinity)
			{
				throw new InvalidOperationException("Infinity is not a valid agreement value for MQV");
			}
			return eCPoint.AffineXCoord.ToBigInteger();
		}

		private static ECPoint CalculateMqvAgreement(ECDomainParameters parameters, ECPrivateKeyParameters d1U, ECPrivateKeyParameters d2U, ECPublicKeyParameters Q2U, ECPublicKeyParameters Q1V, ECPublicKeyParameters Q2V)
		{
			BigInteger n = parameters.N;
			int num = (n.BitLength + 1) / 2;
			BigInteger m = BigInteger.One.ShiftLeft(num);
			ECCurve curve = parameters.Curve;
			ECPoint[] array = new ECPoint[3]
			{
				ECAlgorithms.ImportPoint(curve, (Q2U == null) ? parameters.G.Multiply(d2U.D) : Q2U.Q),
				ECAlgorithms.ImportPoint(curve, Q1V.Q),
				ECAlgorithms.ImportPoint(curve, Q2V.Q)
			};
			curve.NormalizeAll(array);
			ECPoint eCPoint = array[0];
			ECPoint p = array[1];
			ECPoint eCPoint2 = array[2];
			BigInteger bigInteger = eCPoint.AffineXCoord.ToBigInteger();
			BigInteger bigInteger2 = bigInteger.Mod(m);
			BigInteger val = bigInteger2.SetBit(num);
			BigInteger val2 = d1U.D.Multiply(val).Add(d2U.D).Mod(n);
			BigInteger bigInteger3 = eCPoint2.AffineXCoord.ToBigInteger();
			BigInteger bigInteger4 = bigInteger3.Mod(m);
			BigInteger bigInteger5 = bigInteger4.SetBit(num);
			BigInteger bigInteger6 = parameters.H.Multiply(val2).Mod(n);
			return ECAlgorithms.SumOfTwoMultiplies(p, bigInteger5.Multiply(bigInteger6).Mod(n), eCPoint2, bigInteger6);
		}
	}
}
