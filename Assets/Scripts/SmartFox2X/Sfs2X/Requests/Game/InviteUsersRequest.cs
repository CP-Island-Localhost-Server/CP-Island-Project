using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	public class InviteUsersRequest : BaseRequest
	{
		public static readonly string KEY_USER = "u";

		public static readonly string KEY_USER_ID = "ui";

		public static readonly string KEY_INVITATION_ID = "ii";

		public static readonly string KEY_TIME = "t";

		public static readonly string KEY_PARAMS = "p";

		public static readonly string KEY_INVITEE_ID = "ee";

		public static readonly string KEY_INVITED_USERS = "iu";

		public static readonly string KEY_REPLY_ID = "ri";

		public static readonly int MAX_INVITATIONS_FROM_CLIENT_SIDE = 8;

		public static readonly int MIN_EXPIRY_TIME = 5;

		public static readonly int MAX_EXPIRY_TIME = 300;

		private List<object> invitedUsers;

		private int secondsForAnswer;

		private ISFSObject parameters;

		public InviteUsersRequest(List<object> invitedUsers, int secondsForReply, ISFSObject parameters)
			: base(RequestType.InviteUser)
		{
			this.invitedUsers = invitedUsers;
			secondsForAnswer = secondsForReply;
			this.parameters = parameters;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (invitedUsers == null || invitedUsers.Count < 1)
			{
				list.Add("No invitation(s) to send");
			}
			if (invitedUsers.Count > MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				list.Add("Too many invitations. Max allowed from client side is: " + MAX_INVITATIONS_FROM_CLIENT_SIDE);
			}
			if (secondsForAnswer < 5 || secondsForAnswer > 300)
			{
				list.Add("SecondsForAnswer value is out of range (" + MIN_EXPIRY_TIME + "-" + MAX_EXPIRY_TIME + ")");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("InvitationReply request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			List<int> list = new List<int>();
			foreach (object invitedUser in invitedUsers)
			{
				if (invitedUser is User)
				{
					if (invitedUser as User != sfs.MySelf)
					{
						list.Add((invitedUser as User).Id);
					}
				}
				else if (invitedUser is Buddy)
				{
					list.Add((invitedUser as Buddy).Id);
				}
			}
			sfso.PutIntArray(KEY_INVITED_USERS, list.ToArray());
			sfso.PutShort(KEY_TIME, (short)secondsForAnswer);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_PARAMS, parameters);
			}
		}
	}
}
