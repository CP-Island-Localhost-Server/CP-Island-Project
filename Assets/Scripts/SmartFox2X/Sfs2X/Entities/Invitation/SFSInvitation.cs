using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Invitation
{
	public class SFSInvitation : Invitation
	{
		protected int id;

		protected User inviter;

		protected User invitee;

		protected int secondsForAnswer;

		protected ISFSObject parameters;

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

		public User Inviter
		{
			get
			{
				return inviter;
			}
		}

		public User Invitee
		{
			get
			{
				return invitee;
			}
		}

		public int SecondsForAnswer
		{
			get
			{
				return secondsForAnswer;
			}
		}

		public ISFSObject Params
		{
			get
			{
				return parameters;
			}
		}

		public SFSInvitation(User inviter, User invitee)
		{
			Init(inviter, invitee, 15, null);
		}

		public SFSInvitation(User inviter, User invitee, int secondsForAnswer)
		{
			Init(inviter, invitee, secondsForAnswer, null);
		}

		public SFSInvitation(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			Init(inviter, invitee, secondsForAnswer, parameters);
		}

		private void Init(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			this.inviter = inviter;
			this.invitee = invitee;
			this.secondsForAnswer = secondsForAnswer;
			this.parameters = parameters;
		}
	}
}
