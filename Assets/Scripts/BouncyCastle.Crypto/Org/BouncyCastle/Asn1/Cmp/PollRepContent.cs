using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PollRepContent : Asn1Encodable
	{
		private readonly DerInteger certReqId;

		private readonly DerInteger checkAfter;

		private readonly PkiFreeText reason;

		public virtual DerInteger CertReqID
		{
			get
			{
				return certReqId;
			}
		}

		public virtual DerInteger CheckAfter
		{
			get
			{
				return checkAfter;
			}
		}

		public virtual PkiFreeText Reason
		{
			get
			{
				return reason;
			}
		}

		private PollRepContent(Asn1Sequence seq)
		{
			certReqId = DerInteger.GetInstance(seq[0]);
			checkAfter = DerInteger.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				reason = PkiFreeText.GetInstance(seq[2]);
			}
		}

		public static PollRepContent GetInstance(object obj)
		{
			if (obj is PollRepContent)
			{
				return (PollRepContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PollRepContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + Platform.GetTypeName(obj), "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(certReqId, checkAfter);
			asn1EncodableVector.AddOptional(reason);
			return new DerSequence(asn1EncodableVector);
		}
	}
}
