using System;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class CertTemplate : Asn1Encodable
	{
		private readonly Asn1Sequence seq;

		private readonly DerInteger version;

		private readonly DerInteger serialNumber;

		private readonly AlgorithmIdentifier signingAlg;

		private readonly X509Name issuer;

		private readonly OptionalValidity validity;

		private readonly X509Name subject;

		private readonly SubjectPublicKeyInfo publicKey;

		private readonly DerBitString issuerUID;

		private readonly DerBitString subjectUID;

		private readonly X509Extensions extensions;

		public virtual int Version
		{
			get
			{
				return version.Value.IntValue;
			}
		}

		public virtual DerInteger SerialNumber
		{
			get
			{
				return serialNumber;
			}
		}

		public virtual AlgorithmIdentifier SigningAlg
		{
			get
			{
				return signingAlg;
			}
		}

		public virtual X509Name Issuer
		{
			get
			{
				return issuer;
			}
		}

		public virtual OptionalValidity Validity
		{
			get
			{
				return validity;
			}
		}

		public virtual X509Name Subject
		{
			get
			{
				return subject;
			}
		}

		public virtual SubjectPublicKeyInfo PublicKey
		{
			get
			{
				return publicKey;
			}
		}

		public virtual DerBitString IssuerUID
		{
			get
			{
				return issuerUID;
			}
		}

		public virtual DerBitString SubjectUID
		{
			get
			{
				return subjectUID;
			}
		}

		public virtual X509Extensions Extensions
		{
			get
			{
				return extensions;
			}
		}

		private CertTemplate(Asn1Sequence seq)
		{
			this.seq = seq;
			foreach (Asn1TaggedObject item in seq)
			{
				switch (item.TagNo)
				{
				case 0:
					version = DerInteger.GetInstance(item, false);
					break;
				case 1:
					serialNumber = DerInteger.GetInstance(item, false);
					break;
				case 2:
					signingAlg = AlgorithmIdentifier.GetInstance(item, false);
					break;
				case 3:
					issuer = X509Name.GetInstance(item, true);
					break;
				case 4:
					validity = OptionalValidity.GetInstance(Asn1Sequence.GetInstance(item, false));
					break;
				case 5:
					subject = X509Name.GetInstance(item, true);
					break;
				case 6:
					publicKey = SubjectPublicKeyInfo.GetInstance(item, false);
					break;
				case 7:
					issuerUID = DerBitString.GetInstance(item, false);
					break;
				case 8:
					subjectUID = DerBitString.GetInstance(item, false);
					break;
				case 9:
					extensions = X509Extensions.GetInstance(item, false);
					break;
				default:
					throw new ArgumentException("unknown tag: " + item.TagNo, "seq");
				}
			}
		}

		public static CertTemplate GetInstance(object obj)
		{
			if (obj is CertTemplate)
			{
				return (CertTemplate)obj;
			}
			if (obj != null)
			{
				return new CertTemplate(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			return seq;
		}
	}
}
