using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Bcpg
{
	public class LiteralDataPacket : InputStreamPacket
	{
		private int format;

		private byte[] fileName;

		private long modDate;

		public int Format
		{
			get
			{
				return format;
			}
		}

		public long ModificationTime
		{
			get
			{
				return modDate;
			}
		}

		public string FileName
		{
			get
			{
				return Strings.FromUtf8ByteArray(fileName);
			}
		}

		internal LiteralDataPacket(BcpgInputStream bcpgIn)
			: base(bcpgIn)
		{
			format = bcpgIn.ReadByte();
			int num = bcpgIn.ReadByte();
			fileName = new byte[num];
			for (int i = 0; i != num; i++)
			{
				fileName[i] = (byte)bcpgIn.ReadByte();
			}
			modDate = (long)(uint)((bcpgIn.ReadByte() << 24) | (bcpgIn.ReadByte() << 16) | (bcpgIn.ReadByte() << 8) | bcpgIn.ReadByte()) * 1000L;
		}

		public byte[] GetRawFileName()
		{
			return Arrays.Clone(fileName);
		}
	}
}
