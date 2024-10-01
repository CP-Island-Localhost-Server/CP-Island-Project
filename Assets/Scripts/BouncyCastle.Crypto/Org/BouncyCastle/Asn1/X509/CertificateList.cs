using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CertificateList : Asn1Encodable
	{
		private readonly TbsCertificateList tbsCertList;

		private readonly AlgorithmIdentifier sigAlgID;

		private readonly DerBitString sig;

		public TbsCertificateList TbsCertList
		{
			get
			{
				return tbsCertList;
			}
		}

		public AlgorithmIdentifier SignatureAlgorithm
		{
			get
			{
				return sigAlgID;
			}
		}

		public DerBitString Signature
		{
			get
			{
				return sig;
			}
		}

		public int Version
		{
			get
			{
				return tbsCertList.Version;
			}
		}

		public X509Name Issuer
		{
			get
			{
				return tbsCertList.Issuer;
			}
		}

		public Time ThisUpdate
		{
			get
			{
				return tbsCertList.ThisUpdate;
			}
		}

		public Time NextUpdate
		{
			get
			{
				return tbsCertList.NextUpdate;
			}
		}

		public static CertificateList GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static CertificateList GetInstance(object obj)
		{
			if (obj is CertificateList)
			{
				return (CertificateList)obj;
			}
			if (obj != null)
			{
				return new CertificateList(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		private CertificateList(Asn1Sequence seq)
		{
			if (seq.Count != 3)
			{
				throw new ArgumentException("sequence wrong size for CertificateList", "seq");
			}
			tbsCertList = TbsCertificateList.GetInstance(seq[0]);
			sigAlgID = AlgorithmIdentifier.GetInstance(seq[1]);
			sig = DerBitString.GetInstance(seq[2]);
		}

		public CrlEntry[] GetRevokedCertificates()
		{
			return tbsCertList.GetRevokedCertificates();
		}

		public IEnumerable GetRevokedCertificateEnumeration()
		{
			return tbsCertList.GetRevokedCertificateEnumeration();
		}

		public byte[] GetSignatureOctets()
		{
			return sig.GetOctets();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(tbsCertList, sigAlgID, sig);
		}
	}
}
