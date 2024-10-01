using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Pkcs
{
	public sealed class EncryptedPrivateKeyInfoFactory
	{
		private EncryptedPrivateKeyInfoFactory()
		{
		}

		public static EncryptedPrivateKeyInfo CreateEncryptedPrivateKeyInfo(DerObjectIdentifier algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
		{
			return CreateEncryptedPrivateKeyInfo(algorithm.Id, passPhrase, salt, iterationCount, PrivateKeyInfoFactory.CreatePrivateKeyInfo(key));
		}

		public static EncryptedPrivateKeyInfo CreateEncryptedPrivateKeyInfo(string algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
		{
			return CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, PrivateKeyInfoFactory.CreatePrivateKeyInfo(key));
		}

		public static EncryptedPrivateKeyInfo CreateEncryptedPrivateKeyInfo(string algorithm, char[] passPhrase, byte[] salt, int iterationCount, PrivateKeyInfo keyInfo)
		{
			IBufferedCipher bufferedCipher = PbeUtilities.CreateEngine(algorithm) as IBufferedCipher;
			if (bufferedCipher == null)
			{
				throw new Exception("Unknown encryption algorithm: " + algorithm);
			}
			Asn1Encodable asn1Encodable = PbeUtilities.GenerateAlgorithmParameters(algorithm, salt, iterationCount);
			ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(algorithm, passPhrase, asn1Encodable);
			bufferedCipher.Init(true, parameters);
			byte[] encoding = bufferedCipher.DoFinal(keyInfo.GetEncoded());
			DerObjectIdentifier objectIdentifier = PbeUtilities.GetObjectIdentifier(algorithm);
			AlgorithmIdentifier algId = new AlgorithmIdentifier(objectIdentifier, asn1Encodable);
			return new EncryptedPrivateKeyInfo(algId, encoding);
		}
	}
}
