using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class SingleResponse : Asn1Encodable
	{
		private readonly CertID certID;

		private readonly CertStatus certStatus;

		private readonly DerGeneralizedTime thisUpdate;

		private readonly DerGeneralizedTime nextUpdate;

		private readonly X509Extensions singleExtensions;

		public CertID CertId
		{
			get
			{
				return certID;
			}
		}

		public CertStatus CertStatus
		{
			get
			{
				return certStatus;
			}
		}

		public DerGeneralizedTime ThisUpdate
		{
			get
			{
				return thisUpdate;
			}
		}

		public DerGeneralizedTime NextUpdate
		{
			get
			{
				return nextUpdate;
			}
		}

		public X509Extensions SingleExtensions
		{
			get
			{
				return singleExtensions;
			}
		}

		public SingleResponse(CertID certID, CertStatus certStatus, DerGeneralizedTime thisUpdate, DerGeneralizedTime nextUpdate, X509Extensions singleExtensions)
		{
			this.certID = certID;
			this.certStatus = certStatus;
			this.thisUpdate = thisUpdate;
			this.nextUpdate = nextUpdate;
			this.singleExtensions = singleExtensions;
		}

		public SingleResponse(Asn1Sequence seq)
		{
			certID = CertID.GetInstance(seq[0]);
			certStatus = CertStatus.GetInstance(seq[1]);
			thisUpdate = (DerGeneralizedTime)seq[2];
			if (seq.Count > 4)
			{
				nextUpdate = DerGeneralizedTime.GetInstance((Asn1TaggedObject)seq[3], true);
				singleExtensions = X509Extensions.GetInstance((Asn1TaggedObject)seq[4], true);
			}
			else if (seq.Count > 3)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[3];
				if (asn1TaggedObject.TagNo == 0)
				{
					nextUpdate = DerGeneralizedTime.GetInstance(asn1TaggedObject, true);
				}
				else
				{
					singleExtensions = X509Extensions.GetInstance(asn1TaggedObject, true);
				}
			}
		}

		public static SingleResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static SingleResponse GetInstance(object obj)
		{
			if (obj == null || obj is SingleResponse)
			{
				return (SingleResponse)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SingleResponse((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(certID, certStatus, thisUpdate);
			if (nextUpdate != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, nextUpdate));
			}
			if (singleExtensions != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 1, singleExtensions));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
