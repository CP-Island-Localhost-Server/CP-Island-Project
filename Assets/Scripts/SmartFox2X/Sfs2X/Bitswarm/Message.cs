using System.Text;
using Sfs2X.Entities.Data;

namespace Sfs2X.Bitswarm
{
	public class Message : IMessage
	{
		private int id;

		private ISFSObject content;

		private int targetController;

		private bool isEncrypted;

		private bool isUDP;

		private long packetId;

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public ISFSObject Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
			}
		}

		public int TargetController
		{
			get
			{
				return targetController;
			}
			set
			{
				targetController = value;
			}
		}

		public bool IsEncrypted
		{
			get
			{
				return isEncrypted;
			}
			set
			{
				isEncrypted = value;
			}
		}

		public bool IsUDP
		{
			get
			{
				return isUDP;
			}
			set
			{
				isUDP = value;
			}
		}

		public long PacketId
		{
			get
			{
				return packetId;
			}
			set
			{
				packetId = value;
			}
		}

		public Message()
		{
			isEncrypted = false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("{ Message id: " + id + " }\n");
			if (content != null)
			{
				stringBuilder.Append("{ Dump: }\n");
				stringBuilder.Append(content.GetDump());
			}
			return stringBuilder.ToString();
		}
	}
}
