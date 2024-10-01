using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public sealed class Rfc6637Utilities
	{
		private static readonly byte[] ANONYMOUS_SENDER = Hex.Decode("416E6F6E796D6F75732053656E64657220202020");

		private Rfc6637Utilities()
		{
		}

		public static string GetAgreementAlgorithm(PublicKeyPacket pubKeyData)
		{
			ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKeyData.Key;
			switch (eCDHPublicBcpgKey.HashAlgorithm)
			{
			case HashAlgorithmTag.Sha256:
				return "ECCDHwithSHA256CKDF";
			case HashAlgorithmTag.Sha384:
				return "ECCDHwithSHA384CKDF";
			case HashAlgorithmTag.Sha512:
				return "ECCDHwithSHA512CKDF";
			default:
				throw new ArgumentException("Unknown hash algorithm specified: " + eCDHPublicBcpgKey.HashAlgorithm);
			}
		}

		public static DerObjectIdentifier GetKeyEncryptionOID(SymmetricKeyAlgorithmTag algID)
		{
			switch (algID)
			{
			case SymmetricKeyAlgorithmTag.Aes128:
				return NistObjectIdentifiers.IdAes128Wrap;
			case SymmetricKeyAlgorithmTag.Aes192:
				return NistObjectIdentifiers.IdAes192Wrap;
			case SymmetricKeyAlgorithmTag.Aes256:
				return NistObjectIdentifiers.IdAes256Wrap;
			default:
				throw new PgpException("unknown symmetric algorithm ID: " + algID);
			}
		}

		public static int GetKeyLength(SymmetricKeyAlgorithmTag algID)
		{
			switch (algID)
			{
			case SymmetricKeyAlgorithmTag.Aes128:
				return 16;
			case SymmetricKeyAlgorithmTag.Aes192:
				return 24;
			case SymmetricKeyAlgorithmTag.Aes256:
				return 32;
			default:
				throw new PgpException("unknown symmetric algorithm ID: " + algID);
			}
		}

		public static byte[] CreateKey(PublicKeyPacket pubKeyData, ECPoint s)
		{
			byte[] parameters = CreateUserKeyingMaterial(pubKeyData);
			ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKeyData.Key;
			return Kdf(eCDHPublicBcpgKey.HashAlgorithm, s, GetKeyLength(eCDHPublicBcpgKey.SymmetricKeyAlgorithm), parameters);
		}

		public static byte[] CreateUserKeyingMaterial(PublicKeyPacket pubKeyData)
		{
			MemoryStream memoryStream = new MemoryStream();
			ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKeyData.Key;
			byte[] encoded = eCDHPublicBcpgKey.CurveOid.GetEncoded();
			memoryStream.Write(encoded, 1, encoded.Length - 1);
			memoryStream.WriteByte((byte)pubKeyData.Algorithm);
			memoryStream.WriteByte(3);
			memoryStream.WriteByte(1);
			memoryStream.WriteByte((byte)eCDHPublicBcpgKey.HashAlgorithm);
			memoryStream.WriteByte((byte)eCDHPublicBcpgKey.SymmetricKeyAlgorithm);
			memoryStream.Write(ANONYMOUS_SENDER, 0, ANONYMOUS_SENDER.Length);
			byte[] array = PgpPublicKey.CalculateFingerprint(pubKeyData);
			memoryStream.Write(array, 0, array.Length);
			return memoryStream.ToArray();
		}

		private static byte[] Kdf(HashAlgorithmTag digestAlg, ECPoint s, int keyLen, byte[] parameters)
		{
			byte[] encoded = s.XCoord.GetEncoded();
			string digestName = PgpUtilities.GetDigestName(digestAlg);
			IDigest digest = DigestUtilities.GetDigest(digestName);
			digest.Update(0);
			digest.Update(0);
			digest.Update(0);
			digest.Update(1);
			digest.BlockUpdate(encoded, 0, encoded.Length);
			digest.BlockUpdate(parameters, 0, parameters.Length);
			byte[] data = DigestUtilities.DoFinal(digest);
			return Arrays.CopyOfRange(data, 0, keyLen);
		}
	}
}
