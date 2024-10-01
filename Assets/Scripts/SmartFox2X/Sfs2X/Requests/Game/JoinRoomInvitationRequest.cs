using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	public class JoinRoomInvitationRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";

		public static readonly string KEY_EXPIRY_SECONDS = "es";

		public static readonly string KEY_INVITED_NAMES = "in";

		public static readonly string KEY_AS_SPECT = "as";

		public static readonly string KEY_OPTIONAL_PARAMS = "op";

		private Room targetRoom;

		private List<string> invitedUserNames;

		private int expirySeconds;

		private bool asSpectator;

		private ISFSObject parameters;

		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters, int expirySeconds, bool asSpectator)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, parameters, expirySeconds, asSpectator);
		}

		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters, int expirySeconds)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, parameters, expirySeconds, false);
		}

		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, parameters, 30, false);
		}

		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, null, 30, false);
		}

		private void Init(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters, int expirySeconds, bool asSpectator)
		{
			this.targetRoom = targetRoom;
			this.invitedUserNames = invitedUserNames;
			this.expirySeconds = expirySeconds;
			this.asSpectator = asSpectator;
			object obj;
			if (parameters != null)
			{
				obj = parameters;
			}
			else
			{
				obj = new SFSObject();
			}
			this.parameters = (ISFSObject)obj;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (targetRoom == null)
			{
				list.Add("Missing target room");
			}
			else if (invitedUserNames == null || invitedUserNames.Count < 1)
			{
				list.Add("No invitees provided");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("JoinRoomInvitationRequest request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM_ID, targetRoom.Id);
			sfso.PutUtfStringArray(KEY_INVITED_NAMES, invitedUserNames.ToArray());
			sfso.PutSFSObject(KEY_OPTIONAL_PARAMS, parameters);
			sfso.PutInt(KEY_EXPIRY_SECONDS, expirySeconds);
			sfso.PutBool(KEY_AS_SPECT, asSpectator);
		}
	}
}
