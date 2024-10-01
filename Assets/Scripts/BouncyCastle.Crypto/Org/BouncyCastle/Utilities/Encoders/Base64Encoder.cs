using System.IO;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public class Base64Encoder : IEncoder
	{
		protected readonly byte[] encodingTable = new byte[64]
		{
			65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
			75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
			85, 86, 87, 88, 89, 90, 97, 98, 99, 100,
			101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
			111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
			121, 122, 48, 49, 50, 51, 52, 53, 54, 55,
			56, 57, 43, 47
		};

		protected byte padding = 61;

		protected readonly byte[] decodingTable = new byte[128];

		protected void InitialiseDecodingTable()
		{
			Arrays.Fill(decodingTable, byte.MaxValue);
			for (int i = 0; i < encodingTable.Length; i++)
			{
				decodingTable[encodingTable[i]] = (byte)i;
			}
		}

		public Base64Encoder()
		{
			InitialiseDecodingTable();
		}

		public int Encode(byte[] data, int off, int length, Stream outStream)
		{
			int num = length % 3;
			int num2 = length - num;
			for (int i = off; i < off + num2; i += 3)
			{
				int num3 = data[i] & 0xFF;
				int num4 = data[i + 1] & 0xFF;
				int num5 = data[i + 2] & 0xFF;
				outStream.WriteByte(encodingTable[((uint)num3 >> 2) & 0x3F]);
				outStream.WriteByte(encodingTable[((uint)(num3 << 4) | ((uint)num4 >> 4)) & 0x3F]);
				outStream.WriteByte(encodingTable[((uint)(num4 << 2) | ((uint)num5 >> 6)) & 0x3F]);
				outStream.WriteByte(encodingTable[num5 & 0x3F]);
			}
			switch (num)
			{
			case 1:
			{
				int num6 = data[off + num2] & 0xFF;
				int num8 = (num6 >> 2) & 0x3F;
				int num9 = (num6 << 4) & 0x3F;
				outStream.WriteByte(encodingTable[num8]);
				outStream.WriteByte(encodingTable[num9]);
				outStream.WriteByte(padding);
				outStream.WriteByte(padding);
				break;
			}
			case 2:
			{
				int num6 = data[off + num2] & 0xFF;
				int num7 = data[off + num2 + 1] & 0xFF;
				int num8 = (num6 >> 2) & 0x3F;
				int num9 = ((num6 << 4) | (num7 >> 4)) & 0x3F;
				int num10 = (num7 << 2) & 0x3F;
				outStream.WriteByte(encodingTable[num8]);
				outStream.WriteByte(encodingTable[num9]);
				outStream.WriteByte(encodingTable[num10]);
				outStream.WriteByte(padding);
				break;
			}
			}
			return num2 / 3 * 4 + ((num != 0) ? 4 : 0);
		}

		private bool ignore(char c)
		{
			if (c != '\n' && c != '\r' && c != '\t')
			{
				return c == ' ';
			}
			return true;
		}

		public int Decode(byte[] data, int off, int length, Stream outStream)
		{
			int num = 0;
			int num2 = off + length;
			while (num2 > off && ignore((char)data[num2 - 1]))
			{
				num2--;
			}
			int i = off;
			int num3 = num2 - 4;
			for (i = nextI(data, i, num3); i < num3; i = nextI(data, i, num3))
			{
				byte b = decodingTable[data[i++]];
				i = nextI(data, i, num3);
				byte b2 = decodingTable[data[i++]];
				i = nextI(data, i, num3);
				byte b3 = decodingTable[data[i++]];
				i = nextI(data, i, num3);
				byte b4 = decodingTable[data[i++]];
				if ((b | b2 | b3 | b4) >= 128)
				{
					throw new IOException("invalid characters encountered in base64 data");
				}
				outStream.WriteByte((byte)((b << 2) | (b2 >> 4)));
				outStream.WriteByte((byte)((b2 << 4) | (b3 >> 2)));
				outStream.WriteByte((byte)((b3 << 6) | b4));
				num += 3;
			}
			return num + decodeLastBlock(outStream, (char)data[num2 - 4], (char)data[num2 - 3], (char)data[num2 - 2], (char)data[num2 - 1]);
		}

		private int nextI(byte[] data, int i, int finish)
		{
			while (i < finish && ignore((char)data[i]))
			{
				i++;
			}
			return i;
		}

		public int DecodeString(string data, Stream outStream)
		{
			int num = 0;
			int num2 = data.Length;
			while (num2 > 0 && ignore(data[num2 - 1]))
			{
				num2--;
			}
			int i = 0;
			int num3 = num2 - 4;
			for (i = nextI(data, i, num3); i < num3; i = nextI(data, i, num3))
			{
				byte b = decodingTable[(uint)data[i++]];
				i = nextI(data, i, num3);
				byte b2 = decodingTable[(uint)data[i++]];
				i = nextI(data, i, num3);
				byte b3 = decodingTable[(uint)data[i++]];
				i = nextI(data, i, num3);
				byte b4 = decodingTable[(uint)data[i++]];
				if ((b | b2 | b3 | b4) >= 128)
				{
					throw new IOException("invalid characters encountered in base64 data");
				}
				outStream.WriteByte((byte)((b << 2) | (b2 >> 4)));
				outStream.WriteByte((byte)((b2 << 4) | (b3 >> 2)));
				outStream.WriteByte((byte)((b3 << 6) | b4));
				num += 3;
			}
			return num + decodeLastBlock(outStream, data[num2 - 4], data[num2 - 3], data[num2 - 2], data[num2 - 1]);
		}

		private int decodeLastBlock(Stream outStream, char c1, char c2, char c3, char c4)
		{
			if (c3 == padding)
			{
				byte b = decodingTable[(uint)c1];
				byte b2 = decodingTable[(uint)c2];
				if ((b | b2) >= 128)
				{
					throw new IOException("invalid characters encountered at end of base64 data");
				}
				outStream.WriteByte((byte)((b << 2) | (b2 >> 4)));
				return 1;
			}
			if (c4 == padding)
			{
				byte b3 = decodingTable[(uint)c1];
				byte b4 = decodingTable[(uint)c2];
				byte b5 = decodingTable[(uint)c3];
				if ((b3 | b4 | b5) >= 128)
				{
					throw new IOException("invalid characters encountered at end of base64 data");
				}
				outStream.WriteByte((byte)((b3 << 2) | (b4 >> 4)));
				outStream.WriteByte((byte)((b4 << 4) | (b5 >> 2)));
				return 2;
			}
			byte b6 = decodingTable[(uint)c1];
			byte b7 = decodingTable[(uint)c2];
			byte b8 = decodingTable[(uint)c3];
			byte b9 = decodingTable[(uint)c4];
			if ((b6 | b7 | b8 | b9) >= 128)
			{
				throw new IOException("invalid characters encountered at end of base64 data");
			}
			outStream.WriteByte((byte)((b6 << 2) | (b7 >> 4)));
			outStream.WriteByte((byte)((b7 << 4) | (b8 >> 2)));
			outStream.WriteByte((byte)((b8 << 6) | b9));
			return 3;
		}

		private int nextI(string data, int i, int finish)
		{
			while (i < finish && ignore(data[i]))
			{
				i++;
			}
			return i;
		}
	}
}
