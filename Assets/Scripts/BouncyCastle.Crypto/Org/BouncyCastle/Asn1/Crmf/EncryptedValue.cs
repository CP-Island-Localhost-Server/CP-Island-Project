using System;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class EncryptedValue : Asn1Encodable
	{
		private readonly AlgorithmIdentifier intendedAlg;

		private readonly AlgorithmIdentifier symmAlg;

		private readonly DerBitString encSymmKey;

		private readonly AlgorithmIdentifier keyAlg;

		private readonly Asn1OctetString valueHint;

		private readonly DerBitString encValue;

		public virtual AlgorithmIdentifier IntendedAlg
		{
			get
			{
				return intendedAlg;
			}
		}

		public virtual AlgorithmIdentifier SymmAlg
		{
			get
			{
				return symmAlg;
			}
		}

		public virtual DerBitString EncSymmKey
		{
			get
			{
				return encSymmKey;
			}
		}

		public virtual AlgorithmIdentifier KeyAlg
		{
			get
			{
				return keyAlg;
			}
		}

		public virtual Asn1OctetString ValueHint
		{
			get
			{
				return valueHint;
			}
		}

		public virtual DerBitString EncValue
		{
			get
			{
				return encValue;
			}
		}

		private EncryptedValue(Asn1Sequence seq)
		{
			int i;
			for (i = 0; seq[i] is Asn1TaggedObject; i++)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[i];
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					intendedAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 1:
					symmAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 2:
					encSymmKey = DerBitString.GetInstance(asn1TaggedObject, false);
					break;
				case 3:
					keyAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 4:
					valueHint = Asn1OctetString.GetInstance(asn1TaggedObject, false);
					break;
				}
			}
			encValue = DerBitString.GetInstance(seq[i]);
		}

		public static EncryptedValue GetInstance(object obj)
		{
			if (obj is EncryptedValue)
			{
				return (EncryptedValue)obj;
			}
			if (obj != null)
			{
				return new EncryptedValue(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public EncryptedValue(AlgorithmIdentifier intendedAlg, AlgorithmIdentifier symmAlg, DerBitString encSymmKey, AlgorithmIdentifier keyAlg, Asn1OctetString valueHint, DerBitString encValue)
		{
			if (encValue == null)
			{
				throw new ArgumentNullException("encValue");
			}
			this.intendedAlg = intendedAlg;
			this.symmAlg = symmAlg;
			this.encSymmKey = encSymmKey;
			this.keyAlg = keyAlg;
			this.valueHint = valueHint;
			this.encValue = encValue;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			AddOptional(asn1EncodableVector, 0, intendedAlg);
			AddOptional(asn1EncodableVector, 1, symmAlg);
			AddOptional(asn1EncodableVector, 2, encSymmKey);
			AddOptional(asn1EncodableVector, 3, keyAlg);
			AddOptional(asn1EncodableVector, 4, valueHint);
			asn1EncodableVector.Add(encValue);
			return new DerSequence(asn1EncodableVector);
		}

		private void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
		{
			if (obj != null)
			{
				v.Add(new DerTaggedObject(false, tagNo, obj));
			}
		}
	}
}
