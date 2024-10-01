using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class LeaveRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";

		private Room room;

		public LeaveRoomRequest(Room room)
			: base(RequestType.LeaveRoom)
		{
			Init(room);
		}

		public LeaveRoomRequest()
			: base(RequestType.LeaveRoom)
		{
			Init(null);
		}

		private void Init(Room room)
		{
			this.room = room;
		}

		public override void Validate(SmartFox sfs)
		{
			if (sfs.JoinedRooms.Count < 1)
			{
				throw new SFSValidationError("LeaveRoom request error", new string[1] { "You are not joined in any rooms" });
			}
		}

		public override void Execute(SmartFox sfs)
		{
			if (room != null)
			{
				sfso.PutInt(KEY_ROOM_ID, room.Id);
			}
		}
	}
}
