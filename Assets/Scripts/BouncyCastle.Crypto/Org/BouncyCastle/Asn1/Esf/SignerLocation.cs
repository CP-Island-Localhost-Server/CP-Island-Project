using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class SignerLocation : Asn1Encodable
	{
		private DerUtf8String countryName;

		private DerUtf8String localityName;

		private Asn1Sequence postalAddress;

		public DerUtf8String CountryName
		{
			get
			{
				return countryName;
			}
		}

		public DerUtf8String LocalityName
		{
			get
			{
				return localityName;
			}
		}

		public Asn1Sequence PostalAddress
		{
			get
			{
				return postalAddress;
			}
		}

		public SignerLocation(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject item in seq)
			{
				switch (item.TagNo)
				{
				case 0:
					countryName = DerUtf8String.GetInstance(item, true);
					break;
				case 1:
					localityName = DerUtf8String.GetInstance(item, true);
					break;
				case 2:
				{
					bool explicitly = item.IsExplicit();
					postalAddress = Asn1Sequence.GetInstance(item, explicitly);
					if (postalAddress != null && postalAddress.Count > 6)
					{
						throw new ArgumentException("postal address must contain less than 6 strings");
					}
					break;
				}
				default:
					throw new ArgumentException("illegal tag");
				}
			}
		}

		public SignerLocation(DerUtf8String countryName, DerUtf8String localityName, Asn1Sequence postalAddress)
		{
			if (postalAddress != null && postalAddress.Count > 6)
			{
				throw new ArgumentException("postal address must contain less than 6 strings");
			}
			if (countryName != null)
			{
				this.countryName = DerUtf8String.GetInstance(countryName.ToAsn1Object());
			}
			if (localityName != null)
			{
				this.localityName = DerUtf8String.GetInstance(localityName.ToAsn1Object());
			}
			if (postalAddress != null)
			{
				this.postalAddress = (Asn1Sequence)postalAddress.ToAsn1Object();
			}
		}

		public static SignerLocation GetInstance(object obj)
		{
			if (obj == null || obj is SignerLocation)
			{
				return (SignerLocation)obj;
			}
			return new SignerLocation(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			if (countryName != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, countryName));
			}
			if (localityName != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 1, localityName));
			}
			if (postalAddress != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 2, postalAddress));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
