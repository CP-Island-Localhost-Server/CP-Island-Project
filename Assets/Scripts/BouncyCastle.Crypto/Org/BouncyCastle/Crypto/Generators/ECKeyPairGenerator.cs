using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class ECKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
	{
		private readonly string algorithm;

		private ECDomainParameters parameters;

		private DerObjectIdentifier publicKeyParamSet;

		private SecureRandom random;

		public ECKeyPairGenerator()
			: this("EC")
		{
		}

		public ECKeyPairGenerator(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			this.algorithm = ECKeyParameters.VerifyAlgorithmName(algorithm);
		}

		public void Init(KeyGenerationParameters parameters)
		{
			if (parameters is ECKeyGenerationParameters)
			{
				ECKeyGenerationParameters eCKeyGenerationParameters = (ECKeyGenerationParameters)parameters;
				publicKeyParamSet = eCKeyGenerationParameters.PublicKeyParamSet;
				this.parameters = eCKeyGenerationParameters.DomainParameters;
			}
			else
			{
				DerObjectIdentifier oid;
				switch (parameters.Strength)
				{
				case 192:
					oid = X9ObjectIdentifiers.Prime192v1;
					break;
				case 224:
					oid = SecObjectIdentifiers.SecP224r1;
					break;
				case 239:
					oid = X9ObjectIdentifiers.Prime239v1;
					break;
				case 256:
					oid = X9ObjectIdentifiers.Prime256v1;
					break;
				case 384:
					oid = SecObjectIdentifiers.SecP384r1;
					break;
				case 521:
					oid = SecObjectIdentifiers.SecP521r1;
					break;
				default:
					throw new InvalidParameterException("unknown key size.");
				}
				X9ECParameters x9ECParameters = FindECCurveByOid(oid);
				publicKeyParamSet = oid;
				this.parameters = new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
			}
			random = parameters.Random;
			if (random == null)
			{
				random = new SecureRandom();
			}
		}

		public AsymmetricCipherKeyPair GenerateKeyPair()
		{
			BigInteger n = parameters.N;
			int num = n.BitLength >> 2;
			BigInteger bigInteger;
			do
			{
				bigInteger = new BigInteger(n.BitLength, random);
			}
			while (bigInteger.CompareTo(BigInteger.Two) < 0 || bigInteger.CompareTo(n) >= 0 || WNafUtilities.GetNafWeight(bigInteger) < num);
			ECPoint q = CreateBasePointMultiplier().Multiply(parameters.G, bigInteger);
			if (publicKeyParamSet != null)
			{
				return new AsymmetricCipherKeyPair(new ECPublicKeyParameters(algorithm, q, publicKeyParamSet), new ECPrivateKeyParameters(algorithm, bigInteger, publicKeyParamSet));
			}
			return new AsymmetricCipherKeyPair(new ECPublicKeyParameters(algorithm, q, parameters), new ECPrivateKeyParameters(algorithm, bigInteger, parameters));
		}

		protected virtual ECMultiplier CreateBasePointMultiplier()
		{
			return new FixedPointCombMultiplier();
		}

		internal static X9ECParameters FindECCurveByOid(DerObjectIdentifier oid)
		{
			X9ECParameters byOid = CustomNamedCurves.GetByOid(oid);
			if (byOid == null)
			{
				byOid = ECNamedCurveTable.GetByOid(oid);
			}
			return byOid;
		}

		internal static ECPublicKeyParameters GetCorrespondingPublicKey(ECPrivateKeyParameters privKey)
		{
			ECDomainParameters eCDomainParameters = privKey.Parameters;
			ECPoint q = new FixedPointCombMultiplier().Multiply(eCDomainParameters.G, privKey.D);
			if (privKey.PublicKeyParamSet != null)
			{
				return new ECPublicKeyParameters(privKey.AlgorithmName, q, privKey.PublicKeyParamSet);
			}
			return new ECPublicKeyParameters(privKey.AlgorithmName, q, eCDomainParameters);
		}
	}
}
