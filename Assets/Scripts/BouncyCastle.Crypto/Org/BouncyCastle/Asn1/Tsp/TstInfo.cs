using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class TstInfo : Asn1Encodable
	{
		private readonly DerInteger version;

		private readonly DerObjectIdentifier tsaPolicyId;

		private readonly MessageImprint messageImprint;

		private readonly DerInteger serialNumber;

		private readonly DerGeneralizedTime genTime;

		private readonly Accuracy accuracy;

		private readonly DerBoolean ordering;

		private readonly DerInteger nonce;

		private readonly GeneralName tsa;

		private readonly X509Extensions extensions;

		public DerInteger Version
		{
			get
			{
				return version;
			}
		}

		public MessageImprint MessageImprint
		{
			get
			{
				return messageImprint;
			}
		}

		public DerObjectIdentifier Policy
		{
			get
			{
				return tsaPolicyId;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return serialNumber;
			}
		}

		public Accuracy Accuracy
		{
			get
			{
				return accuracy;
			}
		}

		public DerGeneralizedTime GenTime
		{
			get
			{
				return genTime;
			}
		}

		public DerBoolean Ordering
		{
			get
			{
				return ordering;
			}
		}

		public DerInteger Nonce
		{
			get
			{
				return nonce;
			}
		}

		public GeneralName Tsa
		{
			get
			{
				return tsa;
			}
		}

		public X509Extensions Extensions
		{
			get
			{
				return extensions;
			}
		}

		public static TstInfo GetInstance(object o)
		{
			if (o == null || o is TstInfo)
			{
				return (TstInfo)o;
			}
			if (o is Asn1Sequence)
			{
				return new TstInfo((Asn1Sequence)o);
			}
			if (o is Asn1OctetString)
			{
				try
				{
					byte[] octets = ((Asn1OctetString)o).GetOctets();
					return GetInstance(Asn1Object.FromByteArray(octets));
				}
				catch (IOException)
				{
					throw new ArgumentException("Bad object format in 'TstInfo' factory.");
				}
			}
			throw new ArgumentException("Unknown object in 'TstInfo' factory: " + Platform.GetTypeName(o));
		}

		private TstInfo(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			version = DerInteger.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			tsaPolicyId = DerObjectIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			messageImprint = MessageImprint.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			serialNumber = DerInteger.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			genTime = DerGeneralizedTime.GetInstance(enumerator.Current);
			ordering = DerBoolean.False;
			while (enumerator.MoveNext())
			{
				Asn1Object asn1Object = (Asn1Object)enumerator.Current;
				if (asn1Object is Asn1TaggedObject)
				{
					DerTaggedObject derTaggedObject = (DerTaggedObject)asn1Object;
					switch (derTaggedObject.TagNo)
					{
					case 0:
						tsa = GeneralName.GetInstance(derTaggedObject, true);
						break;
					case 1:
						extensions = X509Extensions.GetInstance(derTaggedObject, false);
						break;
					default:
						throw new ArgumentException("Unknown tag value " + derTaggedObject.TagNo);
					}
				}
				if (asn1Object is DerSequence)
				{
					accuracy = Accuracy.GetInstance(asn1Object);
				}
				if (asn1Object is DerBoolean)
				{
					ordering = DerBoolean.GetInstance(asn1Object);
				}
				if (asn1Object is DerInteger)
				{
					nonce = DerInteger.GetInstance(asn1Object);
				}
			}
		}

		public TstInfo(DerObjectIdentifier tsaPolicyId, MessageImprint messageImprint, DerInteger serialNumber, DerGeneralizedTime genTime, Accuracy accuracy, DerBoolean ordering, DerInteger nonce, GeneralName tsa, X509Extensions extensions)
		{
			version = new DerInteger(1);
			this.tsaPolicyId = tsaPolicyId;
			this.messageImprint = messageImprint;
			this.serialNumber = serialNumber;
			this.genTime = genTime;
			this.accuracy = accuracy;
			this.ordering = ordering;
			this.nonce = nonce;
			this.tsa = tsa;
			this.extensions = extensions;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version, tsaPolicyId, messageImprint, serialNumber, genTime);
			if (accuracy != null)
			{
				asn1EncodableVector.Add(accuracy);
			}
			if (ordering != null && ordering.IsTrue)
			{
				asn1EncodableVector.Add(ordering);
			}
			if (nonce != null)
			{
				asn1EncodableVector.Add(nonce);
			}
			if (tsa != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, tsa));
			}
			if (extensions != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 1, extensions));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
