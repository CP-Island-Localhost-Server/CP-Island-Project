using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Invitation;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	public class InvitationReplyRequest : BaseRequest
	{
		public static readonly string KEY_INVITATION_ID = "i";

		public static readonly string KEY_INVITATION_REPLY = "r";

		public static readonly string KEY_INVITATION_PARAMS = "p";

		private Invitation invitation;

		private InvitationReply reply;

		private ISFSObject parameters;

		public InvitationReplyRequest(Invitation invitation, InvitationReply reply, ISFSObject parameters)
			: base(RequestType.InvitationReply)
		{
			Init(invitation, reply, parameters);
		}

		public InvitationReplyRequest(Invitation invitation, InvitationReply reply)
			: base(RequestType.InvitationReply)
		{
			Init(invitation, reply, null);
		}

		private void Init(Invitation invitation, InvitationReply reply, ISFSObject parameters)
		{
			this.invitation = invitation;
			this.reply = reply;
			this.parameters = parameters;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (invitation == null)
			{
				list.Add("Missing invitation object");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("InvitationReply request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_INVITATION_ID, invitation.Id);
			sfso.PutByte(KEY_INVITATION_REPLY, (byte)reply);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_INVITATION_PARAMS, parameters);
			}
		}
	}
}
