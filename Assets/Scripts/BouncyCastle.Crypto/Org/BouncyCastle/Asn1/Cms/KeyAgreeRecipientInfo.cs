using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class KeyAgreeRecipientInfo : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorIdentifierOrKey originator;

		private Asn1OctetString ukm;

		private AlgorithmIdentifier keyEncryptionAlgorithm;

		private Asn1Sequence recipientEncryptedKeys;

		public DerInteger Version
		{
			get
			{
				return version;
			}
		}

		public OriginatorIdentifierOrKey Originator
		{
			get
			{
				return originator;
			}
		}

		public Asn1OctetString UserKeyingMaterial
		{
			get
			{
				return ukm;
			}
		}

		public AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				return keyEncryptionAlgorithm;
			}
		}

		public Asn1Sequence RecipientEncryptedKeys
		{
			get
			{
				return recipientEncryptedKeys;
			}
		}

		public KeyAgreeRecipientInfo(OriginatorIdentifierOrKey originator, Asn1OctetString ukm, AlgorithmIdentifier keyEncryptionAlgorithm, Asn1Sequence recipientEncryptedKeys)
		{
			version = new DerInteger(3);
			this.originator = originator;
			this.ukm = ukm;
			this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this.recipientEncryptedKeys = recipientEncryptedKeys;
		}

		public KeyAgreeRecipientInfo(Asn1Sequence seq)
		{
			int index = 0;
			version = (DerInteger)seq[index++];
			originator = OriginatorIdentifierOrKey.GetInstance((Asn1TaggedObject)seq[index++], true);
			if (seq[index] is Asn1TaggedObject)
			{
				ukm = Asn1OctetString.GetInstance((Asn1TaggedObject)seq[index++], true);
			}
			keyEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[index++]);
			recipientEncryptedKeys = (Asn1Sequence)seq[index++];
		}

		public static KeyAgreeRecipientInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static KeyAgreeRecipientInfo GetInstance(object obj)
		{
			if (obj == null || obj is KeyAgreeRecipientInfo)
			{
				return (KeyAgreeRecipientInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new KeyAgreeRecipientInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Illegal object in KeyAgreeRecipientInfo: " + Platform.GetTypeName(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version, new DerTaggedObject(true, 0, originator));
			if (ukm != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 1, ukm));
			}
			asn1EncodableVector.Add(keyEncryptionAlgorithm, recipientEncryptedKeys);
			return new DerSequence(asn1EncodableVector);
		}
	}
}
