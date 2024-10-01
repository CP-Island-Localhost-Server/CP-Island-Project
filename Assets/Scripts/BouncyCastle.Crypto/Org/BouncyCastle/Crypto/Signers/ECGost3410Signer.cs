using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class ECGost3410Signer : IDsa
	{
		private ECKeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "ECGOST3410";
			}
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			if (forSigning)
			{
				if (parameters is ParametersWithRandom)
				{
					ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
					random = parametersWithRandom.Random;
					parameters = parametersWithRandom.Parameters;
				}
				else
				{
					random = new SecureRandom();
				}
				if (!(parameters is ECPrivateKeyParameters))
				{
					throw new InvalidKeyException("EC private key required for signing");
				}
				key = (ECPrivateKeyParameters)parameters;
			}
			else
			{
				if (!(parameters is ECPublicKeyParameters))
				{
					throw new InvalidKeyException("EC public key required for verification");
				}
				key = (ECPublicKeyParameters)parameters;
			}
		}

		public virtual BigInteger[] GenerateSignature(byte[] message)
		{
			byte[] array = new byte[message.Length];
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = message[array.Length - 1 - i];
			}
			BigInteger val = new BigInteger(1, array);
			ECDomainParameters parameters = key.Parameters;
			BigInteger n = parameters.N;
			BigInteger d = ((ECPrivateKeyParameters)key).D;
			BigInteger bigInteger = null;
			ECMultiplier eCMultiplier = CreateBasePointMultiplier();
			BigInteger bigInteger3;
			while (true)
			{
				BigInteger bigInteger2 = new BigInteger(n.BitLength, random);
				if (bigInteger2.SignValue == 0)
				{
					continue;
				}
				ECPoint eCPoint = eCMultiplier.Multiply(parameters.G, bigInteger2).Normalize();
				bigInteger3 = eCPoint.AffineXCoord.ToBigInteger().Mod(n);
				if (bigInteger3.SignValue != 0)
				{
					bigInteger = bigInteger2.Multiply(val).Add(d.Multiply(bigInteger3)).Mod(n);
					if (bigInteger.SignValue != 0)
					{
						break;
					}
				}
			}
			return new BigInteger[2] { bigInteger3, bigInteger };
		}

		public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			byte[] array = new byte[message.Length];
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = message[array.Length - 1 - i];
			}
			BigInteger bigInteger = new BigInteger(1, array);
			BigInteger n = key.Parameters.N;
			if (r.CompareTo(BigInteger.One) < 0 || r.CompareTo(n) >= 0)
			{
				return false;
			}
			if (s.CompareTo(BigInteger.One) < 0 || s.CompareTo(n) >= 0)
			{
				return false;
			}
			BigInteger val = bigInteger.ModInverse(n);
			BigInteger a = s.Multiply(val).Mod(n);
			BigInteger b = n.Subtract(r).Multiply(val).Mod(n);
			ECPoint g = key.Parameters.G;
			ECPoint q = ((ECPublicKeyParameters)key).Q;
			ECPoint eCPoint = ECAlgorithms.SumOfTwoMultiplies(g, a, q, b).Normalize();
			if (eCPoint.IsInfinity)
			{
				return false;
			}
			BigInteger bigInteger2 = eCPoint.AffineXCoord.ToBigInteger().Mod(n);
			return bigInteger2.Equals(r);
		}

		protected virtual ECMultiplier CreateBasePointMultiplier()
		{
			return new FixedPointCombMultiplier();
		}
	}
}
