namespace Org.BouncyCastle.Asn1.X509
{
	public class TbsCertificateStructure : Asn1Encodable
	{
		internal Asn1Sequence seq;

		internal DerInteger version;

		internal DerInteger serialNumber;

		internal AlgorithmIdentifier signature;

		internal X509Name issuer;

		internal Time startDate;

		internal Time endDate;

		internal X509Name subject;

		internal SubjectPublicKeyInfo subjectPublicKeyInfo;

		internal DerBitString issuerUniqueID;

		internal DerBitString subjectUniqueID;

		internal X509Extensions extensions;

		public int Version
		{
			get
			{
				return version.Value.IntValue + 1;
			}
		}

		public DerInteger VersionNumber
		{
			get
			{
				return version;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return serialNumber;
			}
		}

		public AlgorithmIdentifier Signature
		{
			get
			{
				return signature;
			}
		}

		public X509Name Issuer
		{
			get
			{
				return issuer;
			}
		}

		public Time StartDate
		{
			get
			{
				return startDate;
			}
		}

		public Time EndDate
		{
			get
			{
				return endDate;
			}
		}

		public X509Name Subject
		{
			get
			{
				return subject;
			}
		}

		public SubjectPublicKeyInfo SubjectPublicKeyInfo
		{
			get
			{
				return subjectPublicKeyInfo;
			}
		}

		public DerBitString IssuerUniqueID
		{
			get
			{
				return issuerUniqueID;
			}
		}

		public DerBitString SubjectUniqueID
		{
			get
			{
				return subjectUniqueID;
			}
		}

		public X509Extensions Extensions
		{
			get
			{
				return extensions;
			}
		}

		public static TbsCertificateStructure GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static TbsCertificateStructure GetInstance(object obj)
		{
			if (obj is TbsCertificateStructure)
			{
				return (TbsCertificateStructure)obj;
			}
			if (obj != null)
			{
				return new TbsCertificateStructure(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		internal TbsCertificateStructure(Asn1Sequence seq)
		{
			int num = 0;
			this.seq = seq;
			if (seq[0] is DerTaggedObject)
			{
				version = DerInteger.GetInstance((Asn1TaggedObject)seq[0], true);
			}
			else
			{
				num = -1;
				version = new DerInteger(0);
			}
			serialNumber = DerInteger.GetInstance(seq[num + 1]);
			signature = AlgorithmIdentifier.GetInstance(seq[num + 2]);
			issuer = X509Name.GetInstance(seq[num + 3]);
			Asn1Sequence asn1Sequence = (Asn1Sequence)seq[num + 4];
			startDate = Time.GetInstance(asn1Sequence[0]);
			endDate = Time.GetInstance(asn1Sequence[1]);
			subject = X509Name.GetInstance(seq[num + 5]);
			subjectPublicKeyInfo = SubjectPublicKeyInfo.GetInstance(seq[num + 6]);
			for (int num2 = seq.Count - (num + 6) - 1; num2 > 0; num2--)
			{
				DerTaggedObject derTaggedObject = (DerTaggedObject)seq[num + 6 + num2];
				switch (derTaggedObject.TagNo)
				{
				case 1:
					issuerUniqueID = DerBitString.GetInstance(derTaggedObject, false);
					break;
				case 2:
					subjectUniqueID = DerBitString.GetInstance(derTaggedObject, false);
					break;
				case 3:
					extensions = X509Extensions.GetInstance(derTaggedObject);
					break;
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			return seq;
		}
	}
}
