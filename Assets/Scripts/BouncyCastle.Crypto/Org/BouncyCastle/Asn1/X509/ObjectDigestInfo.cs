using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class ObjectDigestInfo : Asn1Encodable
	{
		public const int PublicKey = 0;

		public const int PublicKeyCert = 1;

		public const int OtherObjectDigest = 2;

		internal readonly DerEnumerated digestedObjectType;

		internal readonly DerObjectIdentifier otherObjectTypeID;

		internal readonly AlgorithmIdentifier digestAlgorithm;

		internal readonly DerBitString objectDigest;

		public DerEnumerated DigestedObjectType
		{
			get
			{
				return digestedObjectType;
			}
		}

		public DerObjectIdentifier OtherObjectTypeID
		{
			get
			{
				return otherObjectTypeID;
			}
		}

		public AlgorithmIdentifier DigestAlgorithm
		{
			get
			{
				return digestAlgorithm;
			}
		}

		public DerBitString ObjectDigest
		{
			get
			{
				return objectDigest;
			}
		}

		public static ObjectDigestInfo GetInstance(object obj)
		{
			if (obj == null || obj is ObjectDigestInfo)
			{
				return (ObjectDigestInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ObjectDigestInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public static ObjectDigestInfo GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public ObjectDigestInfo(int digestedObjectType, string otherObjectTypeID, AlgorithmIdentifier digestAlgorithm, byte[] objectDigest)
		{
			this.digestedObjectType = new DerEnumerated(digestedObjectType);
			if (digestedObjectType == 2)
			{
				this.otherObjectTypeID = new DerObjectIdentifier(otherObjectTypeID);
			}
			this.digestAlgorithm = digestAlgorithm;
			this.objectDigest = new DerBitString(objectDigest);
		}

		private ObjectDigestInfo(Asn1Sequence seq)
		{
			if (seq.Count > 4 || seq.Count < 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			digestedObjectType = DerEnumerated.GetInstance(seq[0]);
			int num = 0;
			if (seq.Count == 4)
			{
				otherObjectTypeID = DerObjectIdentifier.GetInstance(seq[1]);
				num++;
			}
			digestAlgorithm = AlgorithmIdentifier.GetInstance(seq[1 + num]);
			objectDigest = DerBitString.GetInstance(seq[2 + num]);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(digestedObjectType);
			if (otherObjectTypeID != null)
			{
				asn1EncodableVector.Add(otherObjectTypeID);
			}
			asn1EncodableVector.Add(digestAlgorithm, objectDigest);
			return new DerSequence(asn1EncodableVector);
		}
	}
}
