using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class AttributePkcs : Asn1Encodable
	{
		private readonly DerObjectIdentifier attrType;

		private readonly Asn1Set attrValues;

		public DerObjectIdentifier AttrType
		{
			get
			{
				return attrType;
			}
		}

		public Asn1Set AttrValues
		{
			get
			{
				return attrValues;
			}
		}

		public static AttributePkcs GetInstance(object obj)
		{
			AttributePkcs attributePkcs = obj as AttributePkcs;
			if (obj == null || attributePkcs != null)
			{
				return attributePkcs;
			}
			Asn1Sequence asn1Sequence = obj as Asn1Sequence;
			if (asn1Sequence != null)
			{
				return new AttributePkcs(asn1Sequence);
			}
			throw new ArgumentException("Unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		private AttributePkcs(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			attrType = DerObjectIdentifier.GetInstance(seq[0]);
			attrValues = Asn1Set.GetInstance(seq[1]);
		}

		public AttributePkcs(DerObjectIdentifier attrType, Asn1Set attrValues)
		{
			this.attrType = attrType;
			this.attrValues = attrValues;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(attrType, attrValues);
		}
	}
}
