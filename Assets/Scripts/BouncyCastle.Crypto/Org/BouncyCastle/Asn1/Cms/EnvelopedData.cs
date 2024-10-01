using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EnvelopedData : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorInfo originatorInfo;

		private Asn1Set recipientInfos;

		private EncryptedContentInfo encryptedContentInfo;

		private Asn1Set unprotectedAttrs;

		public DerInteger Version
		{
			get
			{
				return version;
			}
		}

		public OriginatorInfo OriginatorInfo
		{
			get
			{
				return originatorInfo;
			}
		}

		public Asn1Set RecipientInfos
		{
			get
			{
				return recipientInfos;
			}
		}

		public EncryptedContentInfo EncryptedContentInfo
		{
			get
			{
				return encryptedContentInfo;
			}
		}

		public Asn1Set UnprotectedAttrs
		{
			get
			{
				return unprotectedAttrs;
			}
		}

		public EnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo encryptedContentInfo, Asn1Set unprotectedAttrs)
		{
			version = new DerInteger(CalculateVersion(originatorInfo, recipientInfos, unprotectedAttrs));
			this.originatorInfo = originatorInfo;
			this.recipientInfos = recipientInfos;
			this.encryptedContentInfo = encryptedContentInfo;
			this.unprotectedAttrs = unprotectedAttrs;
		}

		public EnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo encryptedContentInfo, Attributes unprotectedAttrs)
		{
			version = new DerInteger(CalculateVersion(originatorInfo, recipientInfos, Asn1Set.GetInstance(unprotectedAttrs)));
			this.originatorInfo = originatorInfo;
			this.recipientInfos = recipientInfos;
			this.encryptedContentInfo = encryptedContentInfo;
			this.unprotectedAttrs = Asn1Set.GetInstance(unprotectedAttrs);
		}

		[Obsolete("Use 'GetInstance' instead")]
		public EnvelopedData(Asn1Sequence seq)
		{
			int num = 0;
			version = (DerInteger)seq[num++];
			object obj = seq[num++];
			if (obj is Asn1TaggedObject)
			{
				originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject)obj, false);
				obj = seq[num++];
			}
			recipientInfos = Asn1Set.GetInstance(obj);
			encryptedContentInfo = EncryptedContentInfo.GetInstance(seq[num++]);
			if (seq.Count > num)
			{
				unprotectedAttrs = Asn1Set.GetInstance((Asn1TaggedObject)seq[num], false);
			}
		}

		public static EnvelopedData GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static EnvelopedData GetInstance(object obj)
		{
			if (obj is EnvelopedData)
			{
				return (EnvelopedData)obj;
			}
			if (obj == null)
			{
				return null;
			}
			return new EnvelopedData(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version);
			if (originatorInfo != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, originatorInfo));
			}
			asn1EncodableVector.Add(recipientInfos, encryptedContentInfo);
			if (unprotectedAttrs != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, unprotectedAttrs));
			}
			return new BerSequence(asn1EncodableVector);
		}

		public static int CalculateVersion(OriginatorInfo originatorInfo, Asn1Set recipientInfos, Asn1Set unprotectedAttrs)
		{
			if (originatorInfo != null || unprotectedAttrs != null)
			{
				return 2;
			}
			foreach (object recipientInfo in recipientInfos)
			{
				RecipientInfo instance = RecipientInfo.GetInstance(recipientInfo);
				if (instance.Version.Value.IntValue != 0)
				{
					return 2;
				}
			}
			return 0;
		}
	}
}
