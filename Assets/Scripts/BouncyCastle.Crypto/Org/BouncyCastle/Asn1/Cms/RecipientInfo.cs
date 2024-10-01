using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class RecipientInfo : Asn1Encodable, IAsn1Choice
	{
		internal Asn1Encodable info;

		public DerInteger Version
		{
			get
			{
				if (info is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)info;
					switch (asn1TaggedObject.TagNo)
					{
					case 1:
						return KeyAgreeRecipientInfo.GetInstance(asn1TaggedObject, false).Version;
					case 2:
						return GetKekInfo(asn1TaggedObject).Version;
					case 3:
						return PasswordRecipientInfo.GetInstance(asn1TaggedObject, false).Version;
					case 4:
						return new DerInteger(0);
					default:
						throw new InvalidOperationException("unknown tag");
					}
				}
				return KeyTransRecipientInfo.GetInstance(info).Version;
			}
		}

		public bool IsTagged
		{
			get
			{
				return info is Asn1TaggedObject;
			}
		}

		public Asn1Encodable Info
		{
			get
			{
				if (info is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)info;
					switch (asn1TaggedObject.TagNo)
					{
					case 1:
						return KeyAgreeRecipientInfo.GetInstance(asn1TaggedObject, false);
					case 2:
						return GetKekInfo(asn1TaggedObject);
					case 3:
						return PasswordRecipientInfo.GetInstance(asn1TaggedObject, false);
					case 4:
						return OtherRecipientInfo.GetInstance(asn1TaggedObject, false);
					default:
						throw new InvalidOperationException("unknown tag");
					}
				}
				return KeyTransRecipientInfo.GetInstance(info);
			}
		}

		public RecipientInfo(KeyTransRecipientInfo info)
		{
			this.info = info;
		}

		public RecipientInfo(KeyAgreeRecipientInfo info)
		{
			this.info = new DerTaggedObject(false, 1, info);
		}

		public RecipientInfo(KekRecipientInfo info)
		{
			this.info = new DerTaggedObject(false, 2, info);
		}

		public RecipientInfo(PasswordRecipientInfo info)
		{
			this.info = new DerTaggedObject(false, 3, info);
		}

		public RecipientInfo(OtherRecipientInfo info)
		{
			this.info = new DerTaggedObject(false, 4, info);
		}

		public RecipientInfo(Asn1Object info)
		{
			this.info = info;
		}

		public static RecipientInfo GetInstance(object o)
		{
			if (o == null || o is RecipientInfo)
			{
				return (RecipientInfo)o;
			}
			if (o is Asn1Sequence)
			{
				return new RecipientInfo((Asn1Sequence)o);
			}
			if (o is Asn1TaggedObject)
			{
				return new RecipientInfo((Asn1TaggedObject)o);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(o));
		}

		private KekRecipientInfo GetKekInfo(Asn1TaggedObject o)
		{
			return KekRecipientInfo.GetInstance(o, o.IsExplicit());
		}

		public override Asn1Object ToAsn1Object()
		{
			return info.ToAsn1Object();
		}
	}
}
