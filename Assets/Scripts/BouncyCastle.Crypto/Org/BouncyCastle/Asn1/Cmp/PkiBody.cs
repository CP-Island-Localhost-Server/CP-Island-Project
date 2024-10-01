using System;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiBody : Asn1Encodable, IAsn1Choice
	{
		public const int TYPE_INIT_REQ = 0;

		public const int TYPE_INIT_REP = 1;

		public const int TYPE_CERT_REQ = 2;

		public const int TYPE_CERT_REP = 3;

		public const int TYPE_P10_CERT_REQ = 4;

		public const int TYPE_POPO_CHALL = 5;

		public const int TYPE_POPO_REP = 6;

		public const int TYPE_KEY_UPDATE_REQ = 7;

		public const int TYPE_KEY_UPDATE_REP = 8;

		public const int TYPE_KEY_RECOVERY_REQ = 9;

		public const int TYPE_KEY_RECOVERY_REP = 10;

		public const int TYPE_REVOCATION_REQ = 11;

		public const int TYPE_REVOCATION_REP = 12;

		public const int TYPE_CROSS_CERT_REQ = 13;

		public const int TYPE_CROSS_CERT_REP = 14;

		public const int TYPE_CA_KEY_UPDATE_ANN = 15;

		public const int TYPE_CERT_ANN = 16;

		public const int TYPE_REVOCATION_ANN = 17;

		public const int TYPE_CRL_ANN = 18;

		public const int TYPE_CONFIRM = 19;

		public const int TYPE_NESTED = 20;

		public const int TYPE_GEN_MSG = 21;

		public const int TYPE_GEN_REP = 22;

		public const int TYPE_ERROR = 23;

		public const int TYPE_CERT_CONFIRM = 24;

		public const int TYPE_POLL_REQ = 25;

		public const int TYPE_POLL_REP = 26;

		private int tagNo;

		private Asn1Encodable body;

		public virtual int Type
		{
			get
			{
				return tagNo;
			}
		}

		public virtual Asn1Encodable Content
		{
			get
			{
				return body;
			}
		}

		public static PkiBody GetInstance(object obj)
		{
			if (obj is PkiBody)
			{
				return (PkiBody)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new PkiBody((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("Invalid object: " + Platform.GetTypeName(obj), "obj");
		}

		private PkiBody(Asn1TaggedObject tagged)
		{
			tagNo = tagged.TagNo;
			body = GetBodyForType(tagNo, tagged.GetObject());
		}

		public PkiBody(int type, Asn1Encodable content)
		{
			tagNo = type;
			body = GetBodyForType(type, content);
		}

		private static Asn1Encodable GetBodyForType(int type, Asn1Encodable o)
		{
			switch (type)
			{
			case 0:
				return CertReqMessages.GetInstance(o);
			case 1:
				return CertRepMessage.GetInstance(o);
			case 2:
				return CertReqMessages.GetInstance(o);
			case 3:
				return CertRepMessage.GetInstance(o);
			case 4:
				return CertificationRequest.GetInstance(o);
			case 5:
				return PopoDecKeyChallContent.GetInstance(o);
			case 6:
				return PopoDecKeyRespContent.GetInstance(o);
			case 7:
				return CertReqMessages.GetInstance(o);
			case 8:
				return CertRepMessage.GetInstance(o);
			case 9:
				return CertReqMessages.GetInstance(o);
			case 10:
				return KeyRecRepContent.GetInstance(o);
			case 11:
				return RevReqContent.GetInstance(o);
			case 12:
				return RevRepContent.GetInstance(o);
			case 13:
				return CertReqMessages.GetInstance(o);
			case 14:
				return CertRepMessage.GetInstance(o);
			case 15:
				return CAKeyUpdAnnContent.GetInstance(o);
			case 16:
				return CmpCertificate.GetInstance(o);
			case 17:
				return RevAnnContent.GetInstance(o);
			case 18:
				return CrlAnnContent.GetInstance(o);
			case 19:
				return PkiConfirmContent.GetInstance(o);
			case 20:
				return PkiMessages.GetInstance(o);
			case 21:
				return GenMsgContent.GetInstance(o);
			case 22:
				return GenRepContent.GetInstance(o);
			case 23:
				return ErrorMsgContent.GetInstance(o);
			case 24:
				return CertConfirmContent.GetInstance(o);
			case 25:
				return PollReqContent.GetInstance(o);
			case 26:
				return PollRepContent.GetInstance(o);
			default:
				throw new ArgumentException("unknown tag number: " + type, "type");
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerTaggedObject(true, tagNo, body);
		}
	}
}
