using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	public class CreateSFSGameRequest : BaseRequest
	{
		public static readonly string KEY_IS_PUBLIC = "gip";

		public static readonly string KEY_MIN_PLAYERS = "gmp";

		public static readonly string KEY_INVITED_PLAYERS = "ginp";

		public static readonly string KEY_SEARCHABLE_ROOMS = "gsr";

		public static readonly string KEY_PLAYER_MATCH_EXP = "gpme";

		public static readonly string KEY_SPECTATOR_MATCH_EXP = "gsme";

		public static readonly string KEY_INVITATION_EXPIRY = "gie";

		public static readonly string KEY_LEAVE_ROOM = "glr";

		public static readonly string KEY_NOTIFY_GAME_STARTED = "gns";

		public static readonly string KEY_INVITATION_PARAMS = "ip";

		private CreateRoomRequest createRoomRequest;

		private SFSGameSettings settings;

		public CreateSFSGameRequest(SFSGameSettings settings)
			: base(RequestType.CreateSFSGame)
		{
			this.settings = settings;
			createRoomRequest = new CreateRoomRequest(settings, false, null);
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			try
			{
				createRoomRequest.Validate(sfs);
			}
			catch (SFSValidationError sFSValidationError)
			{
				list = sFSValidationError.Errors;
			}
			if (settings.MinPlayersToStartGame > settings.MaxUsers)
			{
				list.Add("minPlayersToStartGame cannot be greater than maxUsers");
			}
			if (settings.InvitationExpiryTime < InviteUsersRequest.MIN_EXPIRY_TIME || settings.InvitationExpiryTime > InviteUsersRequest.MAX_EXPIRY_TIME)
			{
				list.Add("Expiry time value is out of range (" + InviteUsersRequest.MIN_EXPIRY_TIME + "-" + InviteUsersRequest.MAX_EXPIRY_TIME + ")");
			}
			if (settings.InvitedPlayers != null && settings.InvitedPlayers.Count > InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				list.Add("Cannot invite more than " + InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE + " players from client side");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateSFSGame request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			createRoomRequest.Execute(sfs);
			sfso = createRoomRequest.Message.Content;
			sfso.PutBool(KEY_IS_PUBLIC, settings.IsPublic);
			sfso.PutShort(KEY_MIN_PLAYERS, (short)settings.MinPlayersToStartGame);
			sfso.PutShort(KEY_INVITATION_EXPIRY, (short)settings.InvitationExpiryTime);
			sfso.PutBool(KEY_LEAVE_ROOM, settings.LeaveLastJoinedRoom);
			sfso.PutBool(KEY_NOTIFY_GAME_STARTED, settings.NotifyGameStarted);
			if (settings.PlayerMatchExpression != null)
			{
				sfso.PutSFSArray(KEY_PLAYER_MATCH_EXP, settings.PlayerMatchExpression.ToSFSArray());
			}
			if (settings.SpectatorMatchExpression != null)
			{
				sfso.PutSFSArray(KEY_SPECTATOR_MATCH_EXP, settings.SpectatorMatchExpression.ToSFSArray());
			}
			if (settings.InvitedPlayers != null)
			{
				List<int> list = new List<int>();
				foreach (object invitedPlayer in settings.InvitedPlayers)
				{
					if (invitedPlayer is User)
					{
						list.Add((invitedPlayer as User).Id);
					}
					else if (invitedPlayer is Buddy)
					{
						list.Add((invitedPlayer as Buddy).Id);
					}
				}
				sfso.PutIntArray(KEY_INVITED_PLAYERS, list.ToArray());
			}
			if (settings.SearchableRooms != null)
			{
				sfso.PutUtfStringArray(KEY_SEARCHABLE_ROOMS, settings.SearchableRooms.ToArray());
			}
			if (settings.InvitationParams != null)
			{
				sfso.PutSFSObject(KEY_INVITATION_PARAMS, settings.InvitationParams);
			}
		}
	}
}
