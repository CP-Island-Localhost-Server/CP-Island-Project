using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EncryptedContentInfo : Asn1Encodable
	{
		private DerObjectIdentifier contentType;

		private AlgorithmIdentifier contentEncryptionAlgorithm;

		private Asn1OctetString encryptedContent;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return contentType;
			}
		}

		public AlgorithmIdentifier ContentEncryptionAlgorithm
		{
			get
			{
				return contentEncryptionAlgorithm;
			}
		}

		public Asn1OctetString EncryptedContent
		{
			get
			{
				return encryptedContent;
			}
		}

		public EncryptedContentInfo(DerObjectIdentifier contentType, AlgorithmIdentifier contentEncryptionAlgorithm, Asn1OctetString encryptedContent)
		{
			this.contentType = contentType;
			this.contentEncryptionAlgorithm = contentEncryptionAlgorithm;
			this.encryptedContent = encryptedContent;
		}

		public EncryptedContentInfo(Asn1Sequence seq)
		{
			contentType = (DerObjectIdentifier)seq[0];
			contentEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				encryptedContent = Asn1OctetString.GetInstance((Asn1TaggedObject)seq[2], false);
			}
		}

		public static EncryptedContentInfo GetInstance(object obj)
		{
			if (obj == null || obj is EncryptedContentInfo)
			{
				return (EncryptedContentInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new EncryptedContentInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid EncryptedContentInfo: " + Platform.GetTypeName(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(contentType, contentEncryptionAlgorithm);
			if (encryptedContent != null)
			{
				asn1EncodableVector.Add(new BerTaggedObject(false, 0, encryptedContent));
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
