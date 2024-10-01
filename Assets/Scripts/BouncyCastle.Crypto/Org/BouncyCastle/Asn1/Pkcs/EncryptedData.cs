using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class EncryptedData : Asn1Encodable
	{
		private readonly Asn1Sequence data;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return (DerObjectIdentifier)data[0];
			}
		}

		public AlgorithmIdentifier EncryptionAlgorithm
		{
			get
			{
				return AlgorithmIdentifier.GetInstance(data[1]);
			}
		}

		public Asn1OctetString Content
		{
			get
			{
				if (data.Count == 3)
				{
					DerTaggedObject obj = (DerTaggedObject)data[2];
					return Asn1OctetString.GetInstance(obj, false);
				}
				return null;
			}
		}

		public static EncryptedData GetInstance(object obj)
		{
			if (obj is EncryptedData)
			{
				return (EncryptedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new EncryptedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		private EncryptedData(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			if (((DerInteger)seq[0]).Value.IntValue != 0)
			{
				throw new ArgumentException("sequence not version 0");
			}
			data = (Asn1Sequence)seq[1];
		}

		public EncryptedData(DerObjectIdentifier contentType, AlgorithmIdentifier encryptionAlgorithm, Asn1Encodable content)
		{
			data = new BerSequence(contentType, encryptionAlgorithm.ToAsn1Object(), new BerTaggedObject(false, 0, content));
		}

		public override Asn1Object ToAsn1Object()
		{
			return new BerSequence(new DerInteger(0), data);
		}
	}
}
