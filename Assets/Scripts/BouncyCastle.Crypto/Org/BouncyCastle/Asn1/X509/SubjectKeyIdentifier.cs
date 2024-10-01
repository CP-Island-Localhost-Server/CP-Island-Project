using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class SubjectKeyIdentifier : Asn1Encodable
	{
		private readonly byte[] keyIdentifier;

		public static SubjectKeyIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1OctetString.GetInstance(obj, explicitly));
		}

		public static SubjectKeyIdentifier GetInstance(object obj)
		{
			if (obj is SubjectKeyIdentifier)
			{
				return (SubjectKeyIdentifier)obj;
			}
			if (obj is SubjectPublicKeyInfo)
			{
				return new SubjectKeyIdentifier((SubjectPublicKeyInfo)obj);
			}
			if (obj is Asn1OctetString)
			{
				return new SubjectKeyIdentifier((Asn1OctetString)obj);
			}
			if (obj is X509Extension)
			{
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("Invalid SubjectKeyIdentifier: " + Platform.GetTypeName(obj));
		}

		public SubjectKeyIdentifier(byte[] keyID)
		{
			if (keyID == null)
			{
				throw new ArgumentNullException("keyID");
			}
			keyIdentifier = keyID;
		}

		public SubjectKeyIdentifier(Asn1OctetString keyID)
		{
			keyIdentifier = keyID.GetOctets();
		}

		public SubjectKeyIdentifier(SubjectPublicKeyInfo spki)
		{
			keyIdentifier = GetDigest(spki);
		}

		public byte[] GetKeyIdentifier()
		{
			return keyIdentifier;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerOctetString(keyIdentifier);
		}

		public static SubjectKeyIdentifier CreateSha1KeyIdentifier(SubjectPublicKeyInfo keyInfo)
		{
			return new SubjectKeyIdentifier(keyInfo);
		}

		public static SubjectKeyIdentifier CreateTruncatedSha1KeyIdentifier(SubjectPublicKeyInfo keyInfo)
		{
			byte[] digest = GetDigest(keyInfo);
			byte[] array = new byte[8];
			Array.Copy(digest, digest.Length - 8, array, 0, array.Length);
			byte[] array2;
			(array2 = array)[0] = (byte)(array2[0] & 0xFu);
			(array2 = array)[0] = (byte)(array2[0] | 0x40u);
			return new SubjectKeyIdentifier(array);
		}

		private static byte[] GetDigest(SubjectPublicKeyInfo spki)
		{
			IDigest digest = new Sha1Digest();
			byte[] array = new byte[digest.GetDigestSize()];
			byte[] bytes = spki.PublicKeyData.GetBytes();
			digest.BlockUpdate(bytes, 0, bytes.Length);
			digest.DoFinal(array, 0);
			return array;
		}
	}
}
