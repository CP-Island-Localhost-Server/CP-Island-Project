using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class JoinRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";

		public static readonly string KEY_USER_LIST = "ul";

		public static readonly string KEY_ROOM_NAME = "n";

		public static readonly string KEY_ROOM_ID = "i";

		public static readonly string KEY_PASS = "p";

		public static readonly string KEY_ROOM_TO_LEAVE = "rl";

		public static readonly string KEY_AS_SPECTATOR = "sp";

		private int id = -1;

		private string name;

		private string pass;

		private int? roomIdToLeave;

		private bool asSpectator;

		public JoinRoomRequest(object id, string pass, int? roomIdToLeave, bool asSpectator)
			: base(RequestType.JoinRoom)
		{
			Init(id, pass, roomIdToLeave, asSpectator);
		}

		public JoinRoomRequest(object id, string pass, int? roomIdToLeave)
			: base(RequestType.JoinRoom)
		{
			Init(id, pass, roomIdToLeave, false);
		}

		public JoinRoomRequest(object id, string pass)
			: base(RequestType.JoinRoom)
		{
			Init(id, pass, null, false);
		}

		public JoinRoomRequest(object id)
			: base(RequestType.JoinRoom)
		{
			Init(id, null, null, false);
		}

		private void Init(object id, string pass, int? roomIdToLeave, bool asSpectator)
		{
			if (id is string)
			{
				name = id as string;
			}
			else if (id is int)
			{
				this.id = (int)id;
			}
			else if (id is Room)
			{
				this.id = (id as Room).Id;
			}
			this.pass = pass;
			this.roomIdToLeave = roomIdToLeave;
			this.asSpectator = asSpectator;
		}

		public override void Validate(SmartFox sfs)
		{
			if (id < 0 && name == null)
			{
				throw new SFSValidationError("JoinRoomRequest Error", new string[1] { "Missing Room id or name, you should provide at least one" });
			}
		}

		public override void Execute(SmartFox sfs)
		{
			if (id > -1)
			{
				sfso.PutInt(KEY_ROOM_ID, id);
			}
			else if (name != null)
			{
				sfso.PutUtfString(KEY_ROOM_NAME, name);
			}
			if (pass != null)
			{
				sfso.PutUtfString(KEY_PASS, pass);
			}
			int? num = roomIdToLeave;
			if (num.HasValue)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomIdToLeave.Value);
			}
			if (asSpectator)
			{
				sfso.PutBool(KEY_AS_SPECTATOR, asSpectator);
			}
		}
	}
}
