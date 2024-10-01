using System;
using System.IO;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
	public class Asn1InputStream : FilterStream
	{
		private readonly int limit;

		private readonly byte[][] tmpBuffers;

		internal static int FindLimit(Stream input)
		{
			if (input is LimitedInputStream)
			{
				return ((LimitedInputStream)input).GetRemaining();
			}
			if (input is MemoryStream)
			{
				MemoryStream memoryStream = (MemoryStream)input;
				return (int)(memoryStream.Length - memoryStream.Position);
			}
			return int.MaxValue;
		}

		public Asn1InputStream(Stream inputStream)
			: this(inputStream, FindLimit(inputStream))
		{
		}

		public Asn1InputStream(Stream inputStream, int limit)
			: base(inputStream)
		{
			this.limit = limit;
			tmpBuffers = new byte[16][];
		}

		public Asn1InputStream(byte[] input)
			: this(new MemoryStream(input, false), input.Length)
		{
		}

		private Asn1Object BuildObject(int tag, int tagNo, int length)
		{
			bool flag = (tag & 0x20) != 0;
			DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(s, length);
			if (((uint)tag & 0x40u) != 0)
			{
				return new DerApplicationSpecific(flag, tagNo, definiteLengthInputStream.ToArray());
			}
			if (((uint)tag & 0x80u) != 0)
			{
				return new Asn1StreamParser(definiteLengthInputStream).ReadTaggedObject(flag, tagNo);
			}
			if (flag)
			{
				switch (tagNo)
				{
				case 4:
					return new BerOctetString(BuildDerEncodableVector(definiteLengthInputStream));
				case 16:
					return CreateDerSequence(definiteLengthInputStream);
				case 17:
					return CreateDerSet(definiteLengthInputStream);
				case 8:
					return new DerExternal(BuildDerEncodableVector(definiteLengthInputStream));
				default:
					throw new IOException("unknown tag " + tagNo + " encountered");
				}
			}
			return CreatePrimitiveDerObject(tagNo, definiteLengthInputStream, tmpBuffers);
		}

		internal Asn1EncodableVector BuildEncodableVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			Asn1Object asn1Object;
			while ((asn1Object = ReadObject()) != null)
			{
				asn1EncodableVector.Add(asn1Object);
			}
			return asn1EncodableVector;
		}

		internal virtual Asn1EncodableVector BuildDerEncodableVector(DefiniteLengthInputStream dIn)
		{
			return new Asn1InputStream(dIn).BuildEncodableVector();
		}

		internal virtual DerSequence CreateDerSequence(DefiniteLengthInputStream dIn)
		{
			return DerSequence.FromVector(BuildDerEncodableVector(dIn));
		}

		internal virtual DerSet CreateDerSet(DefiniteLengthInputStream dIn)
		{
			return DerSet.FromVector(BuildDerEncodableVector(dIn), false);
		}

		public Asn1Object ReadObject()
		{
			int num = ReadByte();
			if (num <= 0)
			{
				if (num == 0)
				{
					throw new IOException("unexpected end-of-contents marker");
				}
				return null;
			}
			int num2 = ReadTagNumber(s, num);
			bool flag = (num & 0x20) != 0;
			int num3 = ReadLength(s, limit);
			if (num3 < 0)
			{
				if (!flag)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				IndefiniteLengthInputStream inStream = new IndefiniteLengthInputStream(s, limit);
				Asn1StreamParser parser = new Asn1StreamParser(inStream, limit);
				if (((uint)num & 0x40u) != 0)
				{
					return new BerApplicationSpecificParser(num2, parser).ToAsn1Object();
				}
				if (((uint)num & 0x80u) != 0)
				{
					return new BerTaggedObjectParser(true, num2, parser).ToAsn1Object();
				}
				switch (num2)
				{
				case 4:
					return new BerOctetStringParser(parser).ToAsn1Object();
				case 16:
					return new BerSequenceParser(parser).ToAsn1Object();
				case 17:
					return new BerSetParser(parser).ToAsn1Object();
				case 8:
					return new DerExternalParser(parser).ToAsn1Object();
				default:
					throw new IOException("unknown BER object encountered");
				}
			}
			try
			{
				return BuildObject(num, num2, num3);
			}
			catch (ArgumentException exception)
			{
				throw new Asn1Exception("corrupted stream detected", exception);
			}
		}

		internal static int ReadTagNumber(Stream s, int tag)
		{
			int num = tag & 0x1F;
			if (num == 31)
			{
				num = 0;
				int num2 = s.ReadByte();
				if ((num2 & 0x7F) == 0)
				{
					throw new IOException("Corrupted stream - invalid high tag number found");
				}
				while (num2 >= 0 && ((uint)num2 & 0x80u) != 0)
				{
					num |= num2 & 0x7F;
					num <<= 7;
					num2 = s.ReadByte();
				}
				if (num2 < 0)
				{
					throw new EndOfStreamException("EOF found inside tag value.");
				}
				num |= num2 & 0x7F;
			}
			return num;
		}

		internal static int ReadLength(Stream s, int limit)
		{
			int num = s.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException("EOF found when length expected");
			}
			if (num == 128)
			{
				return -1;
			}
			if (num > 127)
			{
				int num2 = num & 0x7F;
				if (num2 > 4)
				{
					throw new IOException("DER length more than 4 bytes: " + num2);
				}
				num = 0;
				for (int i = 0; i < num2; i++)
				{
					int num3 = s.ReadByte();
					if (num3 < 0)
					{
						throw new EndOfStreamException("EOF found reading length");
					}
					num = (num << 8) + num3;
				}
				if (num < 0)
				{
					throw new IOException("Corrupted stream - negative length found");
				}
				if (num >= limit)
				{
					throw new IOException("Corrupted stream - out of bounds length found");
				}
			}
			return num;
		}

		internal static byte[] GetBuffer(DefiniteLengthInputStream defIn, byte[][] tmpBuffers)
		{
			int remaining = defIn.GetRemaining();
			if (remaining >= tmpBuffers.Length)
			{
				return defIn.ToArray();
			}
			byte[] array = tmpBuffers[remaining];
			if (array == null)
			{
				array = (tmpBuffers[remaining] = new byte[remaining]);
			}
			defIn.ReadAllIntoByteArray(array);
			return array;
		}

		internal static Asn1Object CreatePrimitiveDerObject(int tagNo, DefiniteLengthInputStream defIn, byte[][] tmpBuffers)
		{
			switch (tagNo)
			{
			case 1:
				return DerBoolean.FromOctetString(GetBuffer(defIn, tmpBuffers));
			case 10:
				return DerEnumerated.FromOctetString(GetBuffer(defIn, tmpBuffers));
			case 6:
				return DerObjectIdentifier.FromOctetString(GetBuffer(defIn, tmpBuffers));
			default:
			{
				byte[] array = defIn.ToArray();
				switch (tagNo)
				{
				case 3:
					return DerBitString.FromAsn1Octets(array);
				case 30:
					return new DerBmpString(array);
				case 24:
					return new DerGeneralizedTime(array);
				case 27:
					return new DerGeneralString(array);
				case 25:
					return new DerGraphicString(array);
				case 22:
					return new DerIA5String(array);
				case 2:
					return new DerInteger(array);
				case 5:
					return DerNull.Instance;
				case 18:
					return new DerNumericString(array);
				case 4:
					return new DerOctetString(array);
				case 19:
					return new DerPrintableString(array);
				case 20:
					return new DerT61String(array);
				case 28:
					return new DerUniversalString(array);
				case 23:
					return new DerUtcTime(array);
				case 12:
					return new DerUtf8String(array);
				case 21:
					return new DerVideotexString(array);
				case 26:
					return new DerVisibleString(array);
				default:
					throw new IOException("unknown tag " + tagNo + " encountered");
				}
			}
			}
		}
	}
}
