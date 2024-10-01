using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class CrlID : Asn1Encodable
	{
		private readonly DerIA5String crlUrl;

		private readonly DerInteger crlNum;

		private readonly DerGeneralizedTime crlTime;

		public DerIA5String CrlUrl
		{
			get
			{
				return crlUrl;
			}
		}

		public DerInteger CrlNum
		{
			get
			{
				return crlNum;
			}
		}

		public DerGeneralizedTime CrlTime
		{
			get
			{
				return crlTime;
			}
		}

		public CrlID(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject item in seq)
			{
				switch (item.TagNo)
				{
				case 0:
					crlUrl = DerIA5String.GetInstance(item, true);
					break;
				case 1:
					crlNum = DerInteger.GetInstance(item, true);
					break;
				case 2:
					crlTime = DerGeneralizedTime.GetInstance(item, true);
					break;
				default:
					throw new ArgumentException("unknown tag number: " + item.TagNo);
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			if (crlUrl != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, crlUrl));
			}
			if (crlNum != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 1, crlNum));
			}
			if (crlTime != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 2, crlTime));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
