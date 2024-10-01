using Sfs2X.Entities.Data;

namespace Sfs2X.Bitswarm
{
	public interface IMessage
	{
		int Id { get; set; }

		ISFSObject Content { get; set; }

		int TargetController { get; set; }

		bool IsEncrypted { get; set; }

		bool IsUDP { get; set; }

		long PacketId { get; set; }
	}
}
