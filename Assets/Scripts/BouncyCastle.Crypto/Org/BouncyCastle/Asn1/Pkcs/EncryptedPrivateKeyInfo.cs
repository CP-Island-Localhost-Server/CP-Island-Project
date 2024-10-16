using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class EncryptedPrivateKeyInfo : Asn1Encodable
	{
		private readonly AlgorithmIdentifier algId;

		private readonly Asn1OctetString data;

		public AlgorithmIdentifier EncryptionAlgorithm
		{
			get
			{
				return algId;
			}
		}

		private EncryptedPrivateKeyInfo(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			algId = AlgorithmIdentifier.GetInstance(seq[0]);
			data = Asn1OctetString.GetInstance(seq[1]);
		}

		public EncryptedPrivateKeyInfo(AlgorithmIdentifier algId, byte[] encoding)
		{
			this.algId = algId;
			data = new DerOctetString(encoding);
		}

		public static EncryptedPrivateKeyInfo GetInstance(object obj)
		{
			if (obj is EncryptedPrivateKeyInfo)
			{
				return (EncryptedPrivateKeyInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new EncryptedPrivateKeyInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public byte[] GetEncryptedData()
		{
			return data.GetOctets();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(algId, data);
		}
	}
}
