using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class Holder : Asn1Encodable
	{
		internal readonly IssuerSerial baseCertificateID;

		internal readonly GeneralNames entityName;

		internal readonly ObjectDigestInfo objectDigestInfo;

		private readonly int version;

		public int Version
		{
			get
			{
				return version;
			}
		}

		public IssuerSerial BaseCertificateID
		{
			get
			{
				return baseCertificateID;
			}
		}

		public GeneralNames EntityName
		{
			get
			{
				return entityName;
			}
		}

		public ObjectDigestInfo ObjectDigestInfo
		{
			get
			{
				return objectDigestInfo;
			}
		}

		public static Holder GetInstance(object obj)
		{
			if (obj is Holder)
			{
				return (Holder)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Holder((Asn1Sequence)obj);
			}
			if (obj is Asn1TaggedObject)
			{
				return new Holder((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public Holder(Asn1TaggedObject tagObj)
		{
			switch (tagObj.TagNo)
			{
			case 0:
				baseCertificateID = IssuerSerial.GetInstance(tagObj, false);
				break;
			case 1:
				entityName = GeneralNames.GetInstance(tagObj, false);
				break;
			default:
				throw new ArgumentException("unknown tag in Holder");
			}
			version = 0;
		}

		private Holder(Asn1Sequence seq)
		{
			if (seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			for (int i = 0; i != seq.Count; i++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
				switch (instance.TagNo)
				{
				case 0:
					baseCertificateID = IssuerSerial.GetInstance(instance, false);
					break;
				case 1:
					entityName = GeneralNames.GetInstance(instance, false);
					break;
				case 2:
					objectDigestInfo = ObjectDigestInfo.GetInstance(instance, false);
					break;
				default:
					throw new ArgumentException("unknown tag in Holder");
				}
			}
			version = 1;
		}

		public Holder(IssuerSerial baseCertificateID)
			: this(baseCertificateID, 1)
		{
		}

		public Holder(IssuerSerial baseCertificateID, int version)
		{
			this.baseCertificateID = baseCertificateID;
			this.version = version;
		}

		public Holder(GeneralNames entityName)
			: this(entityName, 1)
		{
		}

		public Holder(GeneralNames entityName, int version)
		{
			this.entityName = entityName;
			this.version = version;
		}

		public Holder(ObjectDigestInfo objectDigestInfo)
		{
			this.objectDigestInfo = objectDigestInfo;
			version = 1;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (version == 1)
			{
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
				if (baseCertificateID != null)
				{
					asn1EncodableVector.Add(new DerTaggedObject(false, 0, baseCertificateID));
				}
				if (entityName != null)
				{
					asn1EncodableVector.Add(new DerTaggedObject(false, 1, entityName));
				}
				if (objectDigestInfo != null)
				{
					asn1EncodableVector.Add(new DerTaggedObject(false, 2, objectDigestInfo));
				}
				return new DerSequence(asn1EncodableVector);
			}
			if (entityName != null)
			{
				return new DerTaggedObject(false, 1, entityName);
			}
			return new DerTaggedObject(false, 0, baseCertificateID);
		}
	}
}
