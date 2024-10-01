using Sfs2X.Core;
using Sfs2X.Util;

namespace Sfs2X.Bitswarm
{
	public class PendingPacket
	{
		private PacketHeader header;

		private ByteArray buffer;

		public PacketHeader Header
		{
			get
			{
				return header;
			}
		}

		public ByteArray Buffer
		{
			get
			{
				return buffer;
			}
			set
			{
				buffer = value;
			}
		}

		public PendingPacket(PacketHeader header)
		{
			this.header = header;
			buffer = new ByteArray();
			buffer.Compressed = header.Compressed;
		}
	}
}
