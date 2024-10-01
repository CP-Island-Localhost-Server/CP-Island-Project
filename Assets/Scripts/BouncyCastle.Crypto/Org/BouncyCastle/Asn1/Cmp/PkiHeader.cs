using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiHeader : Asn1Encodable
	{
		public static readonly GeneralName NULL_NAME = new GeneralName(X509Name.GetInstance(new DerSequence()));

		public static readonly int CMP_1999 = 1;

		public static readonly int CMP_2000 = 2;

		private readonly DerInteger pvno;

		private readonly GeneralName sender;

		private readonly GeneralName recipient;

		private readonly DerGeneralizedTime messageTime;

		private readonly AlgorithmIdentifier protectionAlg;

		private readonly Asn1OctetString senderKID;

		private readonly Asn1OctetString recipKID;

		private readonly Asn1OctetString transactionID;

		private readonly Asn1OctetString senderNonce;

		private readonly Asn1OctetString recipNonce;

		private readonly PkiFreeText freeText;

		private readonly Asn1Sequence generalInfo;

		public virtual DerInteger Pvno
		{
			get
			{
				return pvno;
			}
		}

		public virtual GeneralName Sender
		{
			get
			{
				return sender;
			}
		}

		public virtual GeneralName Recipient
		{
			get
			{
				return recipient;
			}
		}

		public virtual DerGeneralizedTime MessageTime
		{
			get
			{
				return messageTime;
			}
		}

		public virtual AlgorithmIdentifier ProtectionAlg
		{
			get
			{
				return protectionAlg;
			}
		}

		public virtual Asn1OctetString SenderKID
		{
			get
			{
				return senderKID;
			}
		}

		public virtual Asn1OctetString RecipKID
		{
			get
			{
				return recipKID;
			}
		}

		public virtual Asn1OctetString TransactionID
		{
			get
			{
				return transactionID;
			}
		}

		public virtual Asn1OctetString SenderNonce
		{
			get
			{
				return senderNonce;
			}
		}

		public virtual Asn1OctetString RecipNonce
		{
			get
			{
				return recipNonce;
			}
		}

		public virtual PkiFreeText FreeText
		{
			get
			{
				return freeText;
			}
		}

		private PkiHeader(Asn1Sequence seq)
		{
			pvno = DerInteger.GetInstance(seq[0]);
			sender = GeneralName.GetInstance(seq[1]);
			recipient = GeneralName.GetInstance(seq[2]);
			for (int i = 3; i < seq.Count; i++)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[i];
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					messageTime = DerGeneralizedTime.GetInstance(asn1TaggedObject, true);
					break;
				case 1:
					protectionAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
					break;
				case 2:
					senderKID = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 3:
					recipKID = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 4:
					transactionID = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 5:
					senderNonce = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 6:
					recipNonce = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 7:
					freeText = PkiFreeText.GetInstance(asn1TaggedObject, true);
					break;
				case 8:
					generalInfo = Asn1Sequence.GetInstance(asn1TaggedObject, true);
					break;
				default:
					throw new ArgumentException("unknown tag number: " + asn1TaggedObject.TagNo, "seq");
				}
			}
		}

		public static PkiHeader GetInstance(object obj)
		{
			if (obj is PkiHeader)
			{
				return (PkiHeader)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PkiHeader((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + Platform.GetTypeName(obj), "obj");
		}

		public PkiHeader(int pvno, GeneralName sender, GeneralName recipient)
			: this(new DerInteger(pvno), sender, recipient)
		{
		}

		private PkiHeader(DerInteger pvno, GeneralName sender, GeneralName recipient)
		{
			this.pvno = pvno;
			this.sender = sender;
			this.recipient = recipient;
		}

		public virtual InfoTypeAndValue[] GetGeneralInfo()
		{
			if (generalInfo == null)
			{
				return null;
			}
			InfoTypeAndValue[] array = new InfoTypeAndValue[generalInfo.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = InfoTypeAndValue.GetInstance(generalInfo[i]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(pvno, sender, recipient);
			AddOptional(v, 0, messageTime);
			AddOptional(v, 1, protectionAlg);
			AddOptional(v, 2, senderKID);
			AddOptional(v, 3, recipKID);
			AddOptional(v, 4, transactionID);
			AddOptional(v, 5, senderNonce);
			AddOptional(v, 6, recipNonce);
			AddOptional(v, 7, freeText);
			AddOptional(v, 8, generalInfo);
			return new DerSequence(v);
		}

		private static void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
		{
			if (obj != null)
			{
				v.Add(new DerTaggedObject(true, tagNo, obj));
			}
		}
	}
}
