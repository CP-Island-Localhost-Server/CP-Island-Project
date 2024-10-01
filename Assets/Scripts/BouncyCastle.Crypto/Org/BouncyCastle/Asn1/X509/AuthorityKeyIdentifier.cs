using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AuthorityKeyIdentifier : Asn1Encodable
	{
		internal readonly Asn1OctetString keyidentifier;

		internal readonly GeneralNames certissuer;

		internal readonly DerInteger certserno;

		public GeneralNames AuthorityCertIssuer
		{
			get
			{
				return certissuer;
			}
		}

		public BigInteger AuthorityCertSerialNumber
		{
			get
			{
				if (certserno != null)
				{
					return certserno.Value;
				}
				return null;
			}
		}

		public static AuthorityKeyIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static AuthorityKeyIdentifier GetInstance(object obj)
		{
			if (obj is AuthorityKeyIdentifier)
			{
				return (AuthorityKeyIdentifier)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AuthorityKeyIdentifier((Asn1Sequence)obj);
			}
			if (obj is X509Extension)
			{
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		protected internal AuthorityKeyIdentifier(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject item in seq)
			{
				switch (item.TagNo)
				{
				case 0:
					keyidentifier = Asn1OctetString.GetInstance(item, false);
					break;
				case 1:
					certissuer = GeneralNames.GetInstance(item, false);
					break;
				case 2:
					certserno = DerInteger.GetInstance(item, false);
					break;
				default:
					throw new ArgumentException("illegal tag");
				}
			}
		}

		public AuthorityKeyIdentifier(SubjectPublicKeyInfo spki)
		{
			IDigest digest = new Sha1Digest();
			byte[] array = new byte[digest.GetDigestSize()];
			byte[] bytes = spki.PublicKeyData.GetBytes();
			digest.BlockUpdate(bytes, 0, bytes.Length);
			digest.DoFinal(array, 0);
			keyidentifier = new DerOctetString(array);
		}

		public AuthorityKeyIdentifier(SubjectPublicKeyInfo spki, GeneralNames name, BigInteger serialNumber)
		{
			IDigest digest = new Sha1Digest();
			byte[] array = new byte[digest.GetDigestSize()];
			byte[] bytes = spki.PublicKeyData.GetBytes();
			digest.BlockUpdate(bytes, 0, bytes.Length);
			digest.DoFinal(array, 0);
			keyidentifier = new DerOctetString(array);
			certissuer = name;
			certserno = new DerInteger(serialNumber);
		}

		public AuthorityKeyIdentifier(GeneralNames name, BigInteger serialNumber)
		{
			keyidentifier = null;
			certissuer = GeneralNames.GetInstance(name.ToAsn1Object());
			certserno = new DerInteger(serialNumber);
		}

		public AuthorityKeyIdentifier(byte[] keyIdentifier)
		{
			keyidentifier = new DerOctetString(keyIdentifier);
			certissuer = null;
			certserno = null;
		}

		public AuthorityKeyIdentifier(byte[] keyIdentifier, GeneralNames name, BigInteger serialNumber)
		{
			keyidentifier = new DerOctetString(keyIdentifier);
			certissuer = GeneralNames.GetInstance(name.ToAsn1Object());
			certserno = new DerInteger(serialNumber);
		}

		public byte[] GetKeyIdentifier()
		{
			if (keyidentifier != null)
			{
				return keyidentifier.GetOctets();
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			if (keyidentifier != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, keyidentifier));
			}
			if (certissuer != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, certissuer));
			}
			if (certserno != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 2, certserno));
			}
			return new DerSequence(asn1EncodableVector);
		}

		public override string ToString()
		{
			return string.Concat("AuthorityKeyIdentifier: KeyID(", keyidentifier.GetOctets(), ")");
		}
	}
}
