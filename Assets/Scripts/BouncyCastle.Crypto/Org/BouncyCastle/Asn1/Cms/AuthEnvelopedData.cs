using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class AuthEnvelopedData : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorInfo originatorInfo;

		private Asn1Set recipientInfos;

		private EncryptedContentInfo authEncryptedContentInfo;

		private Asn1Set authAttrs;

		private Asn1OctetString mac;

		private Asn1Set unauthAttrs;

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

		public EncryptedContentInfo AuthEncryptedContentInfo
		{
			get
			{
				return authEncryptedContentInfo;
			}
		}

		public Asn1Set AuthAttrs
		{
			get
			{
				return authAttrs;
			}
		}

		public Asn1OctetString Mac
		{
			get
			{
				return mac;
			}
		}

		public Asn1Set UnauthAttrs
		{
			get
			{
				return unauthAttrs;
			}
		}

		public AuthEnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo authEncryptedContentInfo, Asn1Set authAttrs, Asn1OctetString mac, Asn1Set unauthAttrs)
		{
			version = new DerInteger(0);
			this.originatorInfo = originatorInfo;
			this.recipientInfos = recipientInfos;
			this.authEncryptedContentInfo = authEncryptedContentInfo;
			this.authAttrs = authAttrs;
			this.mac = mac;
			this.unauthAttrs = unauthAttrs;
		}

		private AuthEnvelopedData(Asn1Sequence seq)
		{
			int num = 0;
			Asn1Object asn1Object = seq[num++].ToAsn1Object();
			version = (DerInteger)asn1Object;
			asn1Object = seq[num++].ToAsn1Object();
			if (asn1Object is Asn1TaggedObject)
			{
				originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject)asn1Object, false);
				asn1Object = seq[num++].ToAsn1Object();
			}
			recipientInfos = Asn1Set.GetInstance(asn1Object);
			asn1Object = seq[num++].ToAsn1Object();
			authEncryptedContentInfo = EncryptedContentInfo.GetInstance(asn1Object);
			asn1Object = seq[num++].ToAsn1Object();
			if (asn1Object is Asn1TaggedObject)
			{
				authAttrs = Asn1Set.GetInstance((Asn1TaggedObject)asn1Object, false);
				asn1Object = seq[num++].ToAsn1Object();
			}
			mac = Asn1OctetString.GetInstance(asn1Object);
			if (seq.Count > num)
			{
				asn1Object = seq[num++].ToAsn1Object();
				unauthAttrs = Asn1Set.GetInstance((Asn1TaggedObject)asn1Object, false);
			}
		}

		public static AuthEnvelopedData GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static AuthEnvelopedData GetInstance(object obj)
		{
			if (obj == null || obj is AuthEnvelopedData)
			{
				return (AuthEnvelopedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AuthEnvelopedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid AuthEnvelopedData: " + Platform.GetTypeName(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version);
			if (originatorInfo != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, originatorInfo));
			}
			asn1EncodableVector.Add(recipientInfos, authEncryptedContentInfo);
			if (authAttrs != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, authAttrs));
			}
			asn1EncodableVector.Add(mac);
			if (unauthAttrs != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 2, unauthAttrs));
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
