using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class PrivateKeyUsagePeriod : Asn1Encodable
	{
		private DerGeneralizedTime _notBefore;

		private DerGeneralizedTime _notAfter;

		public DerGeneralizedTime NotBefore
		{
			get
			{
				return _notBefore;
			}
		}

		public DerGeneralizedTime NotAfter
		{
			get
			{
				return _notAfter;
			}
		}

		public static PrivateKeyUsagePeriod GetInstance(object obj)
		{
			if (obj is PrivateKeyUsagePeriod)
			{
				return (PrivateKeyUsagePeriod)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PrivateKeyUsagePeriod((Asn1Sequence)obj);
			}
			if (obj is X509Extension)
			{
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		private PrivateKeyUsagePeriod(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject item in seq)
			{
				if (item.TagNo == 0)
				{
					_notBefore = DerGeneralizedTime.GetInstance(item, false);
				}
				else if (item.TagNo == 1)
				{
					_notAfter = DerGeneralizedTime.GetInstance(item, false);
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			if (_notBefore != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, _notBefore));
			}
			if (_notAfter != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, _notAfter));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
