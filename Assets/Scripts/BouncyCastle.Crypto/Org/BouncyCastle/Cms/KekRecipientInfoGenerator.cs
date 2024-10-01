using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Cms
{
	internal class KekRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private KeyParameter keyEncryptionKey;

		private string keyEncryptionKeyOID;

		private KekIdentifier kekIdentifier;

		private AlgorithmIdentifier keyEncryptionAlgorithm;

		internal KekIdentifier KekIdentifier
		{
			set
			{
				kekIdentifier = value;
			}
		}

		internal KeyParameter KeyEncryptionKey
		{
			set
			{
				keyEncryptionKey = value;
				keyEncryptionAlgorithm = DetermineKeyEncAlg(keyEncryptionKeyOID, keyEncryptionKey);
			}
		}

		internal string KeyEncryptionKeyOID
		{
			set
			{
				keyEncryptionKeyOID = value;
			}
		}

		internal KekRecipientInfoGenerator()
		{
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] key = contentEncryptionKey.GetKey();
			IWrapper wrapper = Helper.CreateWrapper(keyEncryptionAlgorithm.Algorithm.Id);
			wrapper.Init(true, new ParametersWithRandom(keyEncryptionKey, random));
			Asn1OctetString encryptedKey = new DerOctetString(wrapper.Wrap(key, 0, key.Length));
			return new RecipientInfo(new KekRecipientInfo(kekIdentifier, keyEncryptionAlgorithm, encryptedKey));
		}

		private static AlgorithmIdentifier DetermineKeyEncAlg(string algorithm, KeyParameter key)
		{
			if (Platform.StartsWith(algorithm, "DES"))
			{
				return new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgCms3DesWrap, DerNull.Instance);
			}
			if (Platform.StartsWith(algorithm, "RC2"))
			{
				return new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgCmsRC2Wrap, new DerInteger(58));
			}
			if (Platform.StartsWith(algorithm, "AES"))
			{
				DerObjectIdentifier algorithm2;
				switch (key.GetKey().Length * 8)
				{
				case 128:
					algorithm2 = NistObjectIdentifiers.IdAes128Wrap;
					break;
				case 192:
					algorithm2 = NistObjectIdentifiers.IdAes192Wrap;
					break;
				case 256:
					algorithm2 = NistObjectIdentifiers.IdAes256Wrap;
					break;
				default:
					throw new ArgumentException("illegal keysize in AES");
				}
				return new AlgorithmIdentifier(algorithm2);
			}
			if (Platform.StartsWith(algorithm, "SEED"))
			{
				return new AlgorithmIdentifier(KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap);
			}
			if (Platform.StartsWith(algorithm, "CAMELLIA"))
			{
				DerObjectIdentifier algorithm3;
				switch (key.GetKey().Length * 8)
				{
				case 128:
					algorithm3 = NttObjectIdentifiers.IdCamellia128Wrap;
					break;
				case 192:
					algorithm3 = NttObjectIdentifiers.IdCamellia192Wrap;
					break;
				case 256:
					algorithm3 = NttObjectIdentifiers.IdCamellia256Wrap;
					break;
				default:
					throw new ArgumentException("illegal keysize in Camellia");
				}
				return new AlgorithmIdentifier(algorithm3);
			}
			throw new ArgumentException("unknown algorithm");
		}
	}
}
