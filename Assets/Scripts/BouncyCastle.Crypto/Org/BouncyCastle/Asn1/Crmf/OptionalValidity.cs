using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class OptionalValidity : Asn1Encodable
	{
		private readonly Time notBefore;

		private readonly Time notAfter;

		public virtual Time NotBefore
		{
			get
			{
				return notBefore;
			}
		}

		public virtual Time NotAfter
		{
			get
			{
				return notAfter;
			}
		}

		private OptionalValidity(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject item in seq)
			{
				if (item.TagNo == 0)
				{
					notBefore = Time.GetInstance(item, true);
				}
				else
				{
					notAfter = Time.GetInstance(item, true);
				}
			}
		}

		public static OptionalValidity GetInstance(object obj)
		{
			if (obj == null || obj is OptionalValidity)
			{
				return (OptionalValidity)obj;
			}
			return new OptionalValidity(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			if (notBefore != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, notBefore));
			}
			if (notAfter != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 1, notAfter));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
