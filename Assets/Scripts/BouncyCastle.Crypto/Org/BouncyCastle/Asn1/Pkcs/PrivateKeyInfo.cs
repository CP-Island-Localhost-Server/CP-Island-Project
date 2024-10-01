using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class PrivateKeyInfo : Asn1Encodable
	{
		private readonly Asn1OctetString privKey;

		private readonly AlgorithmIdentifier algID;

		private readonly Asn1Set attributes;

		public virtual AlgorithmIdentifier PrivateKeyAlgorithm
		{
			get
			{
				return algID;
			}
		}

		[Obsolete("Use 'PrivateKeyAlgorithm' property instead")]
		public virtual AlgorithmIdentifier AlgorithmID
		{
			get
			{
				return algID;
			}
		}

		[Obsolete("Use 'ParsePrivateKey' instead")]
		public virtual Asn1Object PrivateKey
		{
			get
			{
				try
				{
					return ParsePrivateKey();
				}
				catch (IOException)
				{
					throw new InvalidOperationException("unable to parse private key");
				}
			}
		}

		public virtual Asn1Set Attributes
		{
			get
			{
				return attributes;
			}
		}

		public static PrivateKeyInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static PrivateKeyInfo GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is PrivateKeyInfo)
			{
				return (PrivateKeyInfo)obj;
			}
			return new PrivateKeyInfo(Asn1Sequence.GetInstance(obj));
		}

		public PrivateKeyInfo(AlgorithmIdentifier algID, Asn1Encodable privateKey)
			: this(algID, privateKey, null)
		{
		}

		public PrivateKeyInfo(AlgorithmIdentifier algID, Asn1Encodable privateKey, Asn1Set attributes)
		{
			this.algID = algID;
			privKey = new DerOctetString(privateKey.GetEncoded("DER"));
			this.attributes = attributes;
		}

		private PrivateKeyInfo(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			BigInteger value = ((DerInteger)enumerator.Current).Value;
			if (value.IntValue != 0)
			{
				throw new ArgumentException("wrong version for private key info: " + value.IntValue);
			}
			enumerator.MoveNext();
			algID = AlgorithmIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			privKey = Asn1OctetString.GetInstance(enumerator.Current);
			if (enumerator.MoveNext())
			{
				attributes = Asn1Set.GetInstance((Asn1TaggedObject)enumerator.Current, false);
			}
		}

		public virtual Asn1Object ParsePrivateKey()
		{
			return Asn1Object.FromByteArray(privKey.GetOctets());
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new DerInteger(0), algID, privKey);
			if (attributes != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(false, 0, attributes));
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
