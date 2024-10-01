using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class SpectatorToPlayerRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";

		public static readonly string KEY_USER_ID = "u";

		public static readonly string KEY_PLAYER_ID = "p";

		private Room room;

		public SpectatorToPlayerRequest(Room targetRoom)
			: base(RequestType.SpectatorToPlayer)
		{
			Init(targetRoom);
		}

		public SpectatorToPlayerRequest()
			: base(RequestType.SpectatorToPlayer)
		{
			Init(null);
		}

		private void Init(Room targetRoom)
		{
			room = targetRoom;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (sfs.JoinedRooms.Count < 1)
			{
				list.Add("You are not joined in any rooms");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("LeaveRoom request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			sfso.PutInt(KEY_ROOM_ID, room.Id);
		}
	}
}
