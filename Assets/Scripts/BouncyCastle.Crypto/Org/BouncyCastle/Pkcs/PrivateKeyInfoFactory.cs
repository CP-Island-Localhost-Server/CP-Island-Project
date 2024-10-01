using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Pkcs
{
	public sealed class PrivateKeyInfoFactory
	{
		private PrivateKeyInfoFactory()
		{
		}

		public static PrivateKeyInfo CreatePrivateKeyInfo(AsymmetricKeyParameter key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!key.IsPrivate)
			{
				throw new ArgumentException("Public key passed - private key expected", "key");
			}
			if (key is ElGamalPrivateKeyParameters)
			{
				ElGamalPrivateKeyParameters elGamalPrivateKeyParameters = (ElGamalPrivateKeyParameters)key;
				return new PrivateKeyInfo(new AlgorithmIdentifier(OiwObjectIdentifiers.ElGamalAlgorithm, new ElGamalParameter(elGamalPrivateKeyParameters.Parameters.P, elGamalPrivateKeyParameters.Parameters.G).ToAsn1Object()), new DerInteger(elGamalPrivateKeyParameters.X));
			}
			if (key is DsaPrivateKeyParameters)
			{
				DsaPrivateKeyParameters dsaPrivateKeyParameters = (DsaPrivateKeyParameters)key;
				return new PrivateKeyInfo(new AlgorithmIdentifier(X9ObjectIdentifiers.IdDsa, new DsaParameter(dsaPrivateKeyParameters.Parameters.P, dsaPrivateKeyParameters.Parameters.Q, dsaPrivateKeyParameters.Parameters.G).ToAsn1Object()), new DerInteger(dsaPrivateKeyParameters.X));
			}
			if (key is DHPrivateKeyParameters)
			{
				DHPrivateKeyParameters dHPrivateKeyParameters = (DHPrivateKeyParameters)key;
				DHParameter dHParameter = new DHParameter(dHPrivateKeyParameters.Parameters.P, dHPrivateKeyParameters.Parameters.G, dHPrivateKeyParameters.Parameters.L);
				return new PrivateKeyInfo(new AlgorithmIdentifier(dHPrivateKeyParameters.AlgorithmOid, dHParameter.ToAsn1Object()), new DerInteger(dHPrivateKeyParameters.X));
			}
			if (key is RsaKeyParameters)
			{
				AlgorithmIdentifier algID = new AlgorithmIdentifier(PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance);
				RsaPrivateKeyStructure rsaPrivateKeyStructure;
				if (key is RsaPrivateCrtKeyParameters)
				{
					RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)key;
					rsaPrivateKeyStructure = new RsaPrivateKeyStructure(rsaPrivateCrtKeyParameters.Modulus, rsaPrivateCrtKeyParameters.PublicExponent, rsaPrivateCrtKeyParameters.Exponent, rsaPrivateCrtKeyParameters.P, rsaPrivateCrtKeyParameters.Q, rsaPrivateCrtKeyParameters.DP, rsaPrivateCrtKeyParameters.DQ, rsaPrivateCrtKeyParameters.QInv);
				}
				else
				{
					RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)key;
					rsaPrivateKeyStructure = new RsaPrivateKeyStructure(rsaKeyParameters.Modulus, BigInteger.Zero, rsaKeyParameters.Exponent, BigInteger.Zero, BigInteger.Zero, BigInteger.Zero, BigInteger.Zero, BigInteger.Zero);
				}
				return new PrivateKeyInfo(algID, rsaPrivateKeyStructure.ToAsn1Object());
			}
			if (key is ECPrivateKeyParameters)
			{
				ECPrivateKeyParameters eCPrivateKeyParameters = (ECPrivateKeyParameters)key;
				ECDomainParameters parameters = eCPrivateKeyParameters.Parameters;
				int bitLength = parameters.N.BitLength;
				AlgorithmIdentifier algID2;
				ECPrivateKeyStructure privateKey;
				if (eCPrivateKeyParameters.AlgorithmName == "ECGOST3410")
				{
					if (eCPrivateKeyParameters.PublicKeyParamSet == null)
					{
						throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");
					}
					Gost3410PublicKeyAlgParameters parameters2 = new Gost3410PublicKeyAlgParameters(eCPrivateKeyParameters.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet);
					algID2 = new AlgorithmIdentifier(CryptoProObjectIdentifiers.GostR3410x2001, parameters2);
					privateKey = new ECPrivateKeyStructure(bitLength, eCPrivateKeyParameters.D);
				}
				else
				{
					X962Parameters parameters3;
					if (eCPrivateKeyParameters.PublicKeyParamSet == null)
					{
						X9ECParameters ecParameters = new X9ECParameters(parameters.Curve, parameters.G, parameters.N, parameters.H, parameters.GetSeed());
						parameters3 = new X962Parameters(ecParameters);
					}
					else
					{
						parameters3 = new X962Parameters(eCPrivateKeyParameters.PublicKeyParamSet);
					}
					privateKey = new ECPrivateKeyStructure(bitLength, eCPrivateKeyParameters.D, parameters3);
					algID2 = new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, parameters3);
				}
				return new PrivateKeyInfo(algID2, privateKey);
			}
			if (key is Gost3410PrivateKeyParameters)
			{
				Gost3410PrivateKeyParameters gost3410PrivateKeyParameters = (Gost3410PrivateKeyParameters)key;
				if (gost3410PrivateKeyParameters.PublicKeyParamSet == null)
				{
					throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");
				}
				byte[] array = gost3410PrivateKeyParameters.X.ToByteArrayUnsigned();
				byte[] array2 = new byte[array.Length];
				for (int i = 0; i != array2.Length; i++)
				{
					array2[i] = array[array.Length - 1 - i];
				}
				Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters = new Gost3410PublicKeyAlgParameters(gost3410PrivateKeyParameters.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet, null);
				AlgorithmIdentifier algID3 = new AlgorithmIdentifier(CryptoProObjectIdentifiers.GostR3410x94, gost3410PublicKeyAlgParameters.ToAsn1Object());
				return new PrivateKeyInfo(algID3, new DerOctetString(array2));
			}
			throw new ArgumentException("Class provided is not convertible: " + Platform.GetTypeName(key));
		}

		public static PrivateKeyInfo CreatePrivateKeyInfo(char[] passPhrase, EncryptedPrivateKeyInfo encInfo)
		{
			return CreatePrivateKeyInfo(passPhrase, false, encInfo);
		}

		public static PrivateKeyInfo CreatePrivateKeyInfo(char[] passPhrase, bool wrongPkcs12Zero, EncryptedPrivateKeyInfo encInfo)
		{
			AlgorithmIdentifier encryptionAlgorithm = encInfo.EncryptionAlgorithm;
			IBufferedCipher bufferedCipher = PbeUtilities.CreateEngine(encryptionAlgorithm) as IBufferedCipher;
			if (bufferedCipher == null)
			{
				throw new Exception("Unknown encryption algorithm: " + encryptionAlgorithm.Algorithm);
			}
			ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(encryptionAlgorithm, passPhrase, wrongPkcs12Zero);
			bufferedCipher.Init(false, parameters);
			byte[] obj = bufferedCipher.DoFinal(encInfo.GetEncryptedData());
			return PrivateKeyInfo.GetInstance(obj);
		}
	}
}
