using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class ChangeRoomPasswordStateRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";

		public static readonly string KEY_PASS = "p";

		private Room room;

		private string newPass;

		public ChangeRoomPasswordStateRequest(Room room, string newPass)
			: base(RequestType.ChangeRoomPassword)
		{
			this.room = room;
			this.newPass = newPass;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (room == null)
			{
				list.Add("Provided room is null");
			}
			if (newPass == null)
			{
				list.Add("Invalid new room password. It must be a non-null string.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangePassState request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM, room.Id);
			sfso.PutUtfString(KEY_PASS, newPass);
		}
	}
}
