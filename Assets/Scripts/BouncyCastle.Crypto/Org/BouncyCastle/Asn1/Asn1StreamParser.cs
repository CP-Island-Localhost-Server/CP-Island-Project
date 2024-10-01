using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class Asn1StreamParser
	{
		private readonly Stream _in;

		private readonly int _limit;

		private readonly byte[][] tmpBuffers;

		public Asn1StreamParser(Stream inStream)
			: this(inStream, Asn1InputStream.FindLimit(inStream))
		{
		}

		public Asn1StreamParser(Stream inStream, int limit)
		{
			if (!inStream.CanRead)
			{
				throw new ArgumentException("Expected stream to be readable", "inStream");
			}
			_in = inStream;
			_limit = limit;
			tmpBuffers = new byte[16][];
		}

		public Asn1StreamParser(byte[] encoding)
			: this(new MemoryStream(encoding, false), encoding.Length)
		{
		}

		internal IAsn1Convertible ReadIndef(int tagValue)
		{
			switch (tagValue)
			{
			case 8:
				return new DerExternalParser(this);
			case 4:
				return new BerOctetStringParser(this);
			case 16:
				return new BerSequenceParser(this);
			case 17:
				return new BerSetParser(this);
			default:
				throw new Asn1Exception("unknown BER object encountered: 0x" + tagValue.ToString("X"));
			}
		}

		internal IAsn1Convertible ReadImplicit(bool constructed, int tag)
		{
			if (_in is IndefiniteLengthInputStream)
			{
				if (!constructed)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				return ReadIndef(tag);
			}
			if (constructed)
			{
				switch (tag)
				{
				case 17:
					return new DerSetParser(this);
				case 16:
					return new DerSequenceParser(this);
				case 4:
					return new BerOctetStringParser(this);
				}
			}
			else
			{
				switch (tag)
				{
				case 17:
					throw new Asn1Exception("sequences must use constructed encoding (see X.690 8.9.1/8.10.1)");
				case 16:
					throw new Asn1Exception("sets must use constructed encoding (see X.690 8.11.1/8.12.1)");
				case 4:
					return new DerOctetStringParser((DefiniteLengthInputStream)_in);
				}
			}
			throw new Asn1Exception("implicit tagging not implemented");
		}

		internal Asn1Object ReadTaggedObject(bool constructed, int tag)
		{
			if (!constructed)
			{
				DefiniteLengthInputStream definiteLengthInputStream = (DefiniteLengthInputStream)_in;
				return new DerTaggedObject(false, tag, new DerOctetString(definiteLengthInputStream.ToArray()));
			}
			Asn1EncodableVector asn1EncodableVector = ReadVector();
			if (_in is IndefiniteLengthInputStream)
			{
				if (asn1EncodableVector.Count != 1)
				{
					return new BerTaggedObject(false, tag, BerSequence.FromVector(asn1EncodableVector));
				}
				return new BerTaggedObject(true, tag, asn1EncodableVector[0]);
			}
			if (asn1EncodableVector.Count != 1)
			{
				return new DerTaggedObject(false, tag, DerSequence.FromVector(asn1EncodableVector));
			}
			return new DerTaggedObject(true, tag, asn1EncodableVector[0]);
		}

		public virtual IAsn1Convertible ReadObject()
		{
			int num = _in.ReadByte();
			if (num == -1)
			{
				return null;
			}
			Set00Check(false);
			int num2 = Asn1InputStream.ReadTagNumber(_in, num);
			bool flag = (num & 0x20) != 0;
			int num3 = Asn1InputStream.ReadLength(_in, _limit);
			if (num3 < 0)
			{
				if (!flag)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				IndefiniteLengthInputStream inStream = new IndefiniteLengthInputStream(_in, _limit);
				Asn1StreamParser asn1StreamParser = new Asn1StreamParser(inStream, _limit);
				if (((uint)num & 0x40u) != 0)
				{
					return new BerApplicationSpecificParser(num2, asn1StreamParser);
				}
				if (((uint)num & 0x80u) != 0)
				{
					return new BerTaggedObjectParser(true, num2, asn1StreamParser);
				}
				return asn1StreamParser.ReadIndef(num2);
			}
			DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(_in, num3);
			if (((uint)num & 0x40u) != 0)
			{
				return new DerApplicationSpecific(flag, num2, definiteLengthInputStream.ToArray());
			}
			if (((uint)num & 0x80u) != 0)
			{
				return new BerTaggedObjectParser(flag, num2, new Asn1StreamParser(definiteLengthInputStream));
			}
			if (flag)
			{
				switch (num2)
				{
				case 4:
					return new BerOctetStringParser(new Asn1StreamParser(definiteLengthInputStream));
				case 16:
					return new DerSequenceParser(new Asn1StreamParser(definiteLengthInputStream));
				case 17:
					return new DerSetParser(new Asn1StreamParser(definiteLengthInputStream));
				case 8:
					return new DerExternalParser(new Asn1StreamParser(definiteLengthInputStream));
				default:
					throw new IOException("unknown tag " + num2 + " encountered");
				}
			}
			int num4 = num2;
			if (num4 == 4)
			{
				return new DerOctetStringParser(definiteLengthInputStream);
			}
			try
			{
				return Asn1InputStream.CreatePrimitiveDerObject(num2, definiteLengthInputStream, tmpBuffers);
			}
			catch (ArgumentException exception)
			{
				throw new Asn1Exception("corrupted stream detected", exception);
			}
		}

		private void Set00Check(bool enabled)
		{
			if (_in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)_in).SetEofOn00(enabled);
			}
		}

		internal Asn1EncodableVector ReadVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			IAsn1Convertible asn1Convertible;
			while ((asn1Convertible = ReadObject()) != null)
			{
				asn1EncodableVector.Add(asn1Convertible.ToAsn1Object());
			}
			return asn1EncodableVector;
		}
	}
}
