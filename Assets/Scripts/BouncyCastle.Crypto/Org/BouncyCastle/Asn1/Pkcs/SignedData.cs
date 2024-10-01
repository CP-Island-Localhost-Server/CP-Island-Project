using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class SignedData : Asn1Encodable
	{
		private readonly DerInteger version;

		private readonly Asn1Set digestAlgorithms;

		private readonly ContentInfo contentInfo;

		private readonly Asn1Set certificates;

		private readonly Asn1Set crls;

		private readonly Asn1Set signerInfos;

		public DerInteger Version
		{
			get
			{
				return version;
			}
		}

		public Asn1Set DigestAlgorithms
		{
			get
			{
				return digestAlgorithms;
			}
		}

		public ContentInfo ContentInfo
		{
			get
			{
				return contentInfo;
			}
		}

		public Asn1Set Certificates
		{
			get
			{
				return certificates;
			}
		}

		public Asn1Set Crls
		{
			get
			{
				return crls;
			}
		}

		public Asn1Set SignerInfos
		{
			get
			{
				return signerInfos;
			}
		}

		public static SignedData GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			SignedData signedData = obj as SignedData;
			if (signedData != null)
			{
				return signedData;
			}
			return new SignedData(Asn1Sequence.GetInstance(obj));
		}

		public SignedData(DerInteger _version, Asn1Set _digestAlgorithms, ContentInfo _contentInfo, Asn1Set _certificates, Asn1Set _crls, Asn1Set _signerInfos)
		{
			version = _version;
			digestAlgorithms = _digestAlgorithms;
			contentInfo = _contentInfo;
			certificates = _certificates;
			crls = _crls;
			signerInfos = _signerInfos;
		}

		private SignedData(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			version = (DerInteger)enumerator.Current;
			enumerator.MoveNext();
			digestAlgorithms = (Asn1Set)enumerator.Current;
			enumerator.MoveNext();
			contentInfo = ContentInfo.GetInstance(enumerator.Current);
			while (enumerator.MoveNext())
			{
				Asn1Object asn1Object = (Asn1Object)enumerator.Current;
				if (asn1Object is DerTaggedObject)
				{
					DerTaggedObject derTaggedObject = (DerTaggedObject)asn1Object;
					switch (derTaggedObject.TagNo)
					{
					case 0:
						certificates = Asn1Set.GetInstance(derTaggedObject, false);
						break;
					case 1:
						crls = Asn1Set.GetInstance(derTaggedObject, false);
						break;
					default:
						throw new ArgumentException("unknown tag value " + derTaggedObject.TagNo);
					}
				}
				else
				{
					signerInfos = (Asn1Set)asn1Object;
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version, digestAlgorithms, contentInfo);
			if (certificates != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, certificates));
			}
			if (crls != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, crls));
			}
			asn1EncodableVector.Add(signerInfos);
			return new BerSequence(asn1EncodableVector);
		}
	}
}
