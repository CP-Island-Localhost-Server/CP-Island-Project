using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class AuthenticatedData : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorInfo originatorInfo;

		private Asn1Set recipientInfos;

		private AlgorithmIdentifier macAlgorithm;

		private AlgorithmIdentifier digestAlgorithm;

		private ContentInfo encapsulatedContentInfo;

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

		public AlgorithmIdentifier MacAlgorithm
		{
			get
			{
				return macAlgorithm;
			}
		}

		public AlgorithmIdentifier DigestAlgorithm
		{
			get
			{
				return digestAlgorithm;
			}
		}

		public ContentInfo EncapsulatedContentInfo
		{
			get
			{
				return encapsulatedContentInfo;
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

		public AuthenticatedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, AlgorithmIdentifier macAlgorithm, AlgorithmIdentifier digestAlgorithm, ContentInfo encapsulatedContent, Asn1Set authAttrs, Asn1OctetString mac, Asn1Set unauthAttrs)
		{
			if ((digestAlgorithm != null || authAttrs != null) && (digestAlgorithm == null || authAttrs == null))
			{
				throw new ArgumentException("digestAlgorithm and authAttrs must be set together");
			}
			version = new DerInteger(CalculateVersion(originatorInfo));
			this.originatorInfo = originatorInfo;
			this.macAlgorithm = macAlgorithm;
			this.digestAlgorithm = digestAlgorithm;
			this.recipientInfos = recipientInfos;
			encapsulatedContentInfo = encapsulatedContent;
			this.authAttrs = authAttrs;
			this.mac = mac;
			this.unauthAttrs = unauthAttrs;
		}

		private AuthenticatedData(Asn1Sequence seq)
		{
			int num = 0;
			version = (DerInteger)seq[num++];
			Asn1Encodable asn1Encodable = seq[num++];
			if (asn1Encodable is Asn1TaggedObject)
			{
				originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject)asn1Encodable, false);
				asn1Encodable = seq[num++];
			}
			recipientInfos = Asn1Set.GetInstance(asn1Encodable);
			macAlgorithm = AlgorithmIdentifier.GetInstance(seq[num++]);
			asn1Encodable = seq[num++];
			if (asn1Encodable is Asn1TaggedObject)
			{
				digestAlgorithm = AlgorithmIdentifier.GetInstance((Asn1TaggedObject)asn1Encodable, false);
				asn1Encodable = seq[num++];
			}
			encapsulatedContentInfo = ContentInfo.GetInstance(asn1Encodable);
			asn1Encodable = seq[num++];
			if (asn1Encodable is Asn1TaggedObject)
			{
				authAttrs = Asn1Set.GetInstance((Asn1TaggedObject)asn1Encodable, false);
				asn1Encodable = seq[num++];
			}
			mac = Asn1OctetString.GetInstance(asn1Encodable);
			if (seq.Count > num)
			{
				unauthAttrs = Asn1Set.GetInstance((Asn1TaggedObject)seq[num], false);
			}
		}

		public static AuthenticatedData GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static AuthenticatedData GetInstance(object obj)
		{
			if (obj == null || obj is AuthenticatedData)
			{
				return (AuthenticatedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AuthenticatedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid AuthenticatedData: " + Platform.GetTypeName(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version);
			if (originatorInfo != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, originatorInfo));
			}
			asn1EncodableVector.Add(recipientInfos, macAlgorithm);
			if (digestAlgorithm != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, digestAlgorithm));
			}
			asn1EncodableVector.Add(encapsulatedContentInfo);
			if (authAttrs != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 2, authAttrs));
			}
			asn1EncodableVector.Add(mac);
			if (unauthAttrs != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 3, unauthAttrs));
			}
			return new BerSequence(asn1EncodableVector);
		}

		public static int CalculateVersion(OriginatorInfo origInfo)
		{
			if (origInfo == null)
			{
				return 0;
			}
			int result = 0;
			foreach (object certificate in origInfo.Certificates)
			{
				if (certificate is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)certificate;
					if (asn1TaggedObject.TagNo == 2)
					{
						result = 1;
					}
					else if (asn1TaggedObject.TagNo == 3)
					{
						result = 3;
						break;
					}
				}
			}
			foreach (object crl in origInfo.Crls)
			{
				if (crl is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject2 = (Asn1TaggedObject)crl;
					if (asn1TaggedObject2.TagNo == 1)
					{
						result = 3;
						break;
					}
				}
			}
			return result;
		}
	}
}
