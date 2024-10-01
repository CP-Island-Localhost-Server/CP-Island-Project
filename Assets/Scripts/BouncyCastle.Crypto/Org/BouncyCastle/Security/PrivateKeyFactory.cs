using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Security
{
	public sealed class PrivateKeyFactory
	{
		private PrivateKeyFactory()
		{
		}

		public static AsymmetricKeyParameter CreateKey(byte[] privateKeyInfoData)
		{
			return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(privateKeyInfoData)));
		}

		public static AsymmetricKeyParameter CreateKey(Stream inStr)
		{
			return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromStream(inStr)));
		}

		public static AsymmetricKeyParameter CreateKey(PrivateKeyInfo keyInfo)
		{
			AlgorithmIdentifier privateKeyAlgorithm = keyInfo.PrivateKeyAlgorithm;
			DerObjectIdentifier algorithm = privateKeyAlgorithm.Algorithm;
			if (algorithm.Equals(PkcsObjectIdentifiers.RsaEncryption) || algorithm.Equals(X509ObjectIdentifiers.IdEARsa) || algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss) || algorithm.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
			{
				RsaPrivateKeyStructure instance = RsaPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey());
				return new RsaPrivateCrtKeyParameters(instance.Modulus, instance.PublicExponent, instance.PrivateExponent, instance.Prime1, instance.Prime2, instance.Exponent1, instance.Exponent2, instance.Coefficient);
			}
			if (algorithm.Equals(PkcsObjectIdentifiers.DhKeyAgreement))
			{
				DHParameter dHParameter = new DHParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				DerInteger derInteger = (DerInteger)keyInfo.ParsePrivateKey();
				BigInteger l = dHParameter.L;
				int l2 = ((l != null) ? l.IntValue : 0);
				DHParameters parameters = new DHParameters(dHParameter.P, dHParameter.G, null, l2);
				return new DHPrivateKeyParameters(derInteger.Value, parameters, algorithm);
			}
			if (algorithm.Equals(OiwObjectIdentifiers.ElGamalAlgorithm))
			{
				ElGamalParameter elGamalParameter = new ElGamalParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				DerInteger derInteger2 = (DerInteger)keyInfo.ParsePrivateKey();
				return new ElGamalPrivateKeyParameters(derInteger2.Value, new ElGamalParameters(elGamalParameter.P, elGamalParameter.G));
			}
			if (algorithm.Equals(X9ObjectIdentifiers.IdDsa))
			{
				DerInteger derInteger3 = (DerInteger)keyInfo.ParsePrivateKey();
				Asn1Encodable parameters2 = privateKeyAlgorithm.Parameters;
				DsaParameters parameters3 = null;
				if (parameters2 != null)
				{
					DsaParameter instance2 = DsaParameter.GetInstance(parameters2.ToAsn1Object());
					parameters3 = new DsaParameters(instance2.P, instance2.Q, instance2.G);
				}
				return new DsaPrivateKeyParameters(derInteger3.Value, parameters3);
			}
			if (algorithm.Equals(X9ObjectIdentifiers.IdECPublicKey))
			{
				X962Parameters x962Parameters = new X962Parameters(privateKeyAlgorithm.Parameters.ToAsn1Object());
				X9ECParameters x9ECParameters = ((!x962Parameters.IsNamedCurve) ? new X9ECParameters((Asn1Sequence)x962Parameters.Parameters) : ECKeyPairGenerator.FindECCurveByOid((DerObjectIdentifier)x962Parameters.Parameters));
				ECPrivateKeyStructure instance3 = ECPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey());
				BigInteger key = instance3.GetKey();
				if (x962Parameters.IsNamedCurve)
				{
					return new ECPrivateKeyParameters("EC", key, (DerObjectIdentifier)x962Parameters.Parameters);
				}
				ECDomainParameters parameters4 = new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
				return new ECPrivateKeyParameters(key, parameters4);
			}
			if (algorithm.Equals(CryptoProObjectIdentifiers.GostR3410x2001))
			{
				Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters = new Gost3410PublicKeyAlgParameters(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				ECDomainParameters byOid = ECGost3410NamedCurves.GetByOid(gost3410PublicKeyAlgParameters.PublicKeyParamSet);
				if (byOid == null)
				{
					throw new ArgumentException("Unrecognized curve OID for GostR3410x2001 private key");
				}
				Asn1Object asn1Object = keyInfo.ParsePrivateKey();
				ECPrivateKeyStructure eCPrivateKeyStructure = ((!(asn1Object is DerInteger)) ? ECPrivateKeyStructure.GetInstance(asn1Object) : new ECPrivateKeyStructure(byOid.N.BitLength, ((DerInteger)asn1Object).Value));
				return new ECPrivateKeyParameters("ECGOST3410", eCPrivateKeyStructure.GetKey(), gost3410PublicKeyAlgParameters.PublicKeyParamSet);
			}
			if (algorithm.Equals(CryptoProObjectIdentifiers.GostR3410x94))
			{
				Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters2 = new Gost3410PublicKeyAlgParameters(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				DerOctetString derOctetString = (DerOctetString)keyInfo.ParsePrivateKey();
				BigInteger x = new BigInteger(1, Arrays.Reverse(derOctetString.GetOctets()));
				return new Gost3410PrivateKeyParameters(x, gost3410PublicKeyAlgParameters2.PublicKeyParamSet);
			}
			throw new SecurityUtilityException("algorithm identifier in key not recognised");
		}

		public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, EncryptedPrivateKeyInfo encInfo)
		{
			return CreateKey(PrivateKeyInfoFactory.CreatePrivateKeyInfo(passPhrase, encInfo));
		}

		public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, byte[] encryptedPrivateKeyInfoData)
		{
			return DecryptKey(passPhrase, Asn1Object.FromByteArray(encryptedPrivateKeyInfoData));
		}

		public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Stream encryptedPrivateKeyInfoStream)
		{
			return DecryptKey(passPhrase, Asn1Object.FromStream(encryptedPrivateKeyInfoStream));
		}

		private static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Asn1Object asn1Object)
		{
			return DecryptKey(passPhrase, EncryptedPrivateKeyInfo.GetInstance(asn1Object));
		}

		public static byte[] EncryptKey(DerObjectIdentifier algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
		{
			return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
		}

		public static byte[] EncryptKey(string algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
		{
			return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
		}
	}
}
