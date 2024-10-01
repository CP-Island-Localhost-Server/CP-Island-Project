using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class CertificationRequestInfo : Asn1Encodable
	{
		internal DerInteger version = new DerInteger(0);

		internal X509Name subject;

		internal SubjectPublicKeyInfo subjectPKInfo;

		internal Asn1Set attributes;

		public DerInteger Version
		{
			get
			{
				return version;
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
				return subjectPKInfo;
			}
		}

		public Asn1Set Attributes
		{
			get
			{
				return attributes;
			}
		}

		public static CertificationRequestInfo GetInstance(object obj)
		{
			if (obj is CertificationRequestInfo)
			{
				return (CertificationRequestInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertificationRequestInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public CertificationRequestInfo(X509Name subject, SubjectPublicKeyInfo pkInfo, Asn1Set attributes)
		{
			this.subject = subject;
			subjectPKInfo = pkInfo;
			this.attributes = attributes;
			if (subject == null || version == null || subjectPKInfo == null)
			{
				throw new ArgumentException("Not all mandatory fields set in CertificationRequestInfo generator.");
			}
		}

		private CertificationRequestInfo(Asn1Sequence seq)
		{
			version = (DerInteger)seq[0];
			subject = X509Name.GetInstance(seq[1]);
			subjectPKInfo = SubjectPublicKeyInfo.GetInstance(seq[2]);
			if (seq.Count > 3)
			{
				DerTaggedObject obj = (DerTaggedObject)seq[3];
				attributes = Asn1Set.GetInstance(obj, false);
			}
			if (subject == null || version == null || subjectPKInfo == null)
			{
				throw new ArgumentException("Not all mandatory fields set in CertificationRequestInfo generator.");
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version, subject, subjectPKInfo);
			if (attributes != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, attributes));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
