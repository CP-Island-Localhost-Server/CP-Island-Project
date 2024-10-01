using System;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class OobCertHash : Asn1Encodable
	{
		private readonly AlgorithmIdentifier hashAlg;

		private readonly CertId certId;

		private readonly DerBitString hashVal;

		public virtual AlgorithmIdentifier HashAlg
		{
			get
			{
				return hashAlg;
			}
		}

		public virtual CertId CertID
		{
			get
			{
				return certId;
			}
		}

		private OobCertHash(Asn1Sequence seq)
		{
			int num = seq.Count - 1;
			hashVal = DerBitString.GetInstance(seq[num--]);
			for (int num2 = num; num2 >= 0; num2--)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[num2];
				if (asn1TaggedObject.TagNo == 0)
				{
					hashAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
				}
				else
				{
					certId = CertId.GetInstance(asn1TaggedObject, true);
				}
			}
		}

		public static OobCertHash GetInstance(object obj)
		{
			if (obj is OobCertHash)
			{
				return (OobCertHash)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OobCertHash((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + Platform.GetTypeName(obj), "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			AddOptional(asn1EncodableVector, 0, hashAlg);
			AddOptional(asn1EncodableVector, 1, certId);
			asn1EncodableVector.Add(hashVal);
			return new DerSequence(asn1EncodableVector);
		}

		private void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
		{
			if (obj != null)
			{
				v.Add(new DerTaggedObject(true, tagNo, obj));
			}
		}
	}
}
