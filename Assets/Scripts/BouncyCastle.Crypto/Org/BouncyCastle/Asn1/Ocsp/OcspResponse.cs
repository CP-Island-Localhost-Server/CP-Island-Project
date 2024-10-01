using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class OcspResponse : Asn1Encodable
	{
		private readonly OcspResponseStatus responseStatus;

		private readonly ResponseBytes responseBytes;

		public OcspResponseStatus ResponseStatus
		{
			get
			{
				return responseStatus;
			}
		}

		public ResponseBytes ResponseBytes
		{
			get
			{
				return responseBytes;
			}
		}

		public static OcspResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static OcspResponse GetInstance(object obj)
		{
			if (obj == null || obj is OcspResponse)
			{
				return (OcspResponse)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OcspResponse((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public OcspResponse(OcspResponseStatus responseStatus, ResponseBytes responseBytes)
		{
			if (responseStatus == null)
			{
				throw new ArgumentNullException("responseStatus");
			}
			this.responseStatus = responseStatus;
			this.responseBytes = responseBytes;
		}

		private OcspResponse(Asn1Sequence seq)
		{
			responseStatus = new OcspResponseStatus(DerEnumerated.GetInstance(seq[0]));
			if (seq.Count == 2)
			{
				responseBytes = ResponseBytes.GetInstance((Asn1TaggedObject)seq[1], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(responseStatus);
			if (responseBytes != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(true, 0, responseBytes));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
