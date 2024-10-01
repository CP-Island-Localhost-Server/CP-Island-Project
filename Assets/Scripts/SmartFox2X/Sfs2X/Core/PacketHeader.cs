using System.Text;

namespace Sfs2X.Core
{
	public class PacketHeader
	{
		private int expectedLength = -1;

		private bool binary = true;

		private bool compressed;

		private bool encrypted;

		private bool blueBoxed;

		private bool bigSized;

		public int ExpectedLength
		{
			get
			{
				return expectedLength;
			}
			set
			{
				expectedLength = value;
			}
		}

		public bool Encrypted
		{
			get
			{
				return encrypted;
			}
			set
			{
				encrypted = value;
			}
		}

		public bool Compressed
		{
			get
			{
				return compressed;
			}
			set
			{
				compressed = value;
			}
		}

		public bool BlueBoxed
		{
			get
			{
				return blueBoxed;
			}
			set
			{
				blueBoxed = value;
			}
		}

		public bool Binary
		{
			get
			{
				return binary;
			}
			set
			{
				binary = value;
			}
		}

		public bool BigSized
		{
			get
			{
				return bigSized;
			}
			set
			{
				bigSized = value;
			}
		}

		public PacketHeader(bool encrypted, bool compressed, bool blueBoxed, bool bigSized)
		{
			this.compressed = compressed;
			this.encrypted = encrypted;
			this.blueBoxed = blueBoxed;
			this.bigSized = bigSized;
		}

		public static PacketHeader FromBinary(int headerByte)
		{
			return new PacketHeader((headerByte & 0x40) > 0, (headerByte & 0x20) > 0, (headerByte & 0x10) > 0, (headerByte & 8) > 0);
		}

		public byte Encode()
		{
			byte b = 0;
			if (binary)
			{
				b = (byte)(b | 0x80u);
			}
			if (Encrypted)
			{
				b = (byte)(b | 0x40u);
			}
			if (Compressed)
			{
				b = (byte)(b | 0x20u);
			}
			if (blueBoxed)
			{
				b = (byte)(b | 0x10u);
			}
			if (bigSized)
			{
				b = (byte)(b | 8u);
			}
			return b;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("---------------------------------------------\n");
			stringBuilder.Append("Binary:  \t" + binary + "\n");
			stringBuilder.Append("Compressed:\t" + compressed + "\n");
			stringBuilder.Append("Encrypted:\t" + encrypted + "\n");
			stringBuilder.Append("BlueBoxed:\t" + blueBoxed + "\n");
			stringBuilder.Append("BigSized:\t" + bigSized + "\n");
			stringBuilder.Append("---------------------------------------------\n");
			return stringBuilder.ToString();
		}
	}
}
