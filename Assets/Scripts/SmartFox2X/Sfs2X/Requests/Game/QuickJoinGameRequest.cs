using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	public class QuickJoinGameRequest : BaseRequest
	{
		private static readonly int MAX_ROOMS = 32;

		public static readonly string KEY_ROOM_LIST = "rl";

		public static readonly string KEY_GROUP_LIST = "gl";

		public static readonly string KEY_ROOM_TO_LEAVE = "tl";

		public static readonly string KEY_MATCH_EXPRESSION = "me";

		private List<Room> whereToSearchRoom;

		private List<string> whereToSearchString;

		private MatchExpression matchExpression;

		private Room roomToLeave;

		private bool isSearchListString = false;

		private bool isSearchListRoom = false;

		public QuickJoinGameRequest(MatchExpression matchExpression, List<string> whereToSearch, Room roomToLeave)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, roomToLeave);
		}

		public QuickJoinGameRequest(MatchExpression matchExpression, List<string> whereToSearch)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, null);
		}

		public QuickJoinGameRequest(MatchExpression matchExpression, List<Room> whereToSearch, Room roomToLeave)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, roomToLeave);
		}

		public QuickJoinGameRequest(MatchExpression matchExpression, List<Room> whereToSearch)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, null);
		}

		private void Init(MatchExpression matchExpression, List<string> whereToSearch, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			whereToSearchString = whereToSearch;
			this.roomToLeave = roomToLeave;
			isSearchListString = true;
		}

		private void Init(MatchExpression matchExpression, List<Room> whereToSearch, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			whereToSearchRoom = whereToSearch;
			this.roomToLeave = roomToLeave;
			isSearchListRoom = true;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (isSearchListRoom)
			{
				if (whereToSearchRoom == null || whereToSearchRoom.Count < 1)
				{
					list.Add("Missing whereToSearch parameter");
				}
				else if (whereToSearchRoom.Count > MAX_ROOMS)
				{
					list.Add("Too many Rooms specified in the whereToSearch parameter. Client limit is: " + MAX_ROOMS);
				}
			}
			if (isSearchListString)
			{
				if (whereToSearchString == null || whereToSearchString.Count < 1)
				{
					list.Add("Missing whereToSearch parameter");
				}
				else if (whereToSearchString.Count > MAX_ROOMS)
				{
					list.Add("Too many Rooms specified in the whereToSearch parameter. Client limit is: " + MAX_ROOMS);
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("QuickJoinGame request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			if (isSearchListString)
			{
				sfso.PutUtfStringArray(KEY_GROUP_LIST, whereToSearchString.ToArray());
			}
			else if (isSearchListRoom)
			{
				List<int> list = new List<int>();
				foreach (Room item in whereToSearchRoom)
				{
					list.Add(item.Id);
				}
				sfso.PutIntArray(KEY_ROOM_LIST, list.ToArray());
			}
			if (roomToLeave != null)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomToLeave.Id);
			}
			if (matchExpression != null)
			{
				sfso.PutSFSArray(KEY_MATCH_EXPRESSION, matchExpression.ToSFSArray());
			}
		}
	}
}
