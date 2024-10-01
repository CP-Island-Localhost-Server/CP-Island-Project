using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Invitation
{
	public interface Invitation
	{
		int Id { get; set; }

		User Inviter { get; }

		User Invitee { get; }

		int SecondsForAnswer { get; }

		ISFSObject Params { get; }
	}
}
