namespace Org.BouncyCastle.Asn1.Cms
{
	public class ScvpReqRes : Asn1Encodable
	{
		private readonly ContentInfo request;

		private readonly ContentInfo response;

		public virtual ContentInfo Request
		{
			get
			{
				return request;
			}
		}

		public virtual ContentInfo Response
		{
			get
			{
				return response;
			}
		}

		public static ScvpReqRes GetInstance(object obj)
		{
			if (obj is ScvpReqRes)
			{
				return (ScvpReqRes)obj;
			}
			if (obj != null)
			{
				return new ScvpReqRes(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		private ScvpReqRes(Asn1Sequence seq)
		{
			if (seq[0] is Asn1TaggedObject)
			{
				request = ContentInfo.GetInstance(Asn1TaggedObject.GetInstance(seq[0]), true);
				response = ContentInfo.GetInstance(seq[1]);
			}
			else
			{
				request = null;
				response = ContentInfo.GetInstance(seq[0]);
			}
		}

		public ScvpReqRes(ContentInfo response)
			: this(null, response)
		{
		}

		public ScvpReqRes(ContentInfo request, ContentInfo response)
		{
			this.request = request;
			this.response = response;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			if (request != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, request));
			}
			asn1EncodableVector.Add(response);
			return new DerSequence(asn1EncodableVector);
		}
	}
}
