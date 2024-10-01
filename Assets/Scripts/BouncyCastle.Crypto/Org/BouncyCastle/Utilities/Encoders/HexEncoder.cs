using System.IO;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public class HexEncoder : IEncoder
	{
		protected readonly byte[] encodingTable = new byte[16]
		{
			48, 49, 50, 51, 52, 53, 54, 55, 56, 57,
			97, 98, 99, 100, 101, 102
		};

		protected readonly byte[] decodingTable = new byte[128];

		protected void InitialiseDecodingTable()
		{
			Arrays.Fill(decodingTable, byte.MaxValue);
			for (int i = 0; i < encodingTable.Length; i++)
			{
				decodingTable[encodingTable[i]] = (byte)i;
			}
			decodingTable[65] = decodingTable[97];
			decodingTable[66] = decodingTable[98];
			decodingTable[67] = decodingTable[99];
			decodingTable[68] = decodingTable[100];
			decodingTable[69] = decodingTable[101];
			decodingTable[70] = decodingTable[102];
		}

		public HexEncoder()
		{
			InitialiseDecodingTable();
		}

		public int Encode(byte[] data, int off, int length, Stream outStream)
		{
			for (int i = off; i < off + length; i++)
			{
				int num = data[i];
				outStream.WriteByte(encodingTable[num >> 4]);
				outStream.WriteByte(encodingTable[num & 0xF]);
			}
			return length * 2;
		}

		private static bool Ignore(char c)
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
			while (num2 > off && Ignore((char)data[num2 - 1]))
			{
				num2--;
			}
			int i = off;
			while (i < num2)
			{
				for (; i < num2 && Ignore((char)data[i]); i++)
				{
				}
				byte b = decodingTable[data[i++]];
				for (; i < num2 && Ignore((char)data[i]); i++)
				{
				}
				byte b2 = decodingTable[data[i++]];
				if ((b | b2) >= 128)
				{
					throw new IOException("invalid characters encountered in Hex data");
				}
				outStream.WriteByte((byte)((b << 4) | b2));
				num++;
			}
			return num;
		}

		public int DecodeString(string data, Stream outStream)
		{
			int num = 0;
			int num2 = data.Length;
			while (num2 > 0 && Ignore(data[num2 - 1]))
			{
				num2--;
			}
			int i = 0;
			while (i < num2)
			{
				for (; i < num2 && Ignore(data[i]); i++)
				{
				}
				byte b = decodingTable[(uint)data[i++]];
				for (; i < num2 && Ignore(data[i]); i++)
				{
				}
				byte b2 = decodingTable[(uint)data[i++]];
				if ((b | b2) >= 128)
				{
					throw new IOException("invalid characters encountered in Hex data");
				}
				outStream.WriteByte((byte)((b << 4) | b2));
				num++;
			}
			return num;
		}
	}
}
