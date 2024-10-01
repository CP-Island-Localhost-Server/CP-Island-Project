using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Match;

namespace Sfs2X.Requests.Game
{
	public class SFSGameSettings : RoomSettings
	{
		private bool isPublic;

		private int minPlayersToStartGame;

		private List<object> invitedPlayers;

		private List<string> searchableRooms;

		private MatchExpression playerMatchExpression;

		private MatchExpression spectatorMatchExpression;

		private int invitationExpiryTime;

		private bool leaveJoinedLastRoom;

		private bool notifyGameStarted;

		private ISFSObject invitationParams;

		public bool IsPublic
		{
			get
			{
				return isPublic;
			}
			set
			{
				isPublic = value;
			}
		}

		public int MinPlayersToStartGame
		{
			get
			{
				return minPlayersToStartGame;
			}
			set
			{
				minPlayersToStartGame = value;
			}
		}

		public List<object> InvitedPlayers
		{
			get
			{
				return invitedPlayers;
			}
			set
			{
				invitedPlayers = value;
			}
		}

		public List<string> SearchableRooms
		{
			get
			{
				return searchableRooms;
			}
			set
			{
				searchableRooms = value;
			}
		}

		public int InvitationExpiryTime
		{
			get
			{
				return invitationExpiryTime;
			}
			set
			{
				invitationExpiryTime = value;
			}
		}

		public bool LeaveLastJoinedRoom
		{
			get
			{
				return leaveJoinedLastRoom;
			}
			set
			{
				leaveJoinedLastRoom = value;
			}
		}

		public bool NotifyGameStarted
		{
			get
			{
				return notifyGameStarted;
			}
			set
			{
				notifyGameStarted = value;
			}
		}

		public MatchExpression PlayerMatchExpression
		{
			get
			{
				return playerMatchExpression;
			}
			set
			{
				playerMatchExpression = value;
			}
		}

		public MatchExpression SpectatorMatchExpression
		{
			get
			{
				return spectatorMatchExpression;
			}
			set
			{
				spectatorMatchExpression = value;
			}
		}

		public ISFSObject InvitationParams
		{
			get
			{
				return invitationParams;
			}
			set
			{
				invitationParams = value;
			}
		}

		public SFSGameSettings(string name)
			: base(name)
		{
			isPublic = true;
			minPlayersToStartGame = 2;
			invitationExpiryTime = 15;
			leaveJoinedLastRoom = true;
			invitedPlayers = new List<object>();
			searchableRooms = new List<string>();
		}
	}
}
