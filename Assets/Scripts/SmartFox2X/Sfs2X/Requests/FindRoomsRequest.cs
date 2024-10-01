using System.Collections.Generic;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class FindRoomsRequest : BaseRequest
	{
		public static readonly string KEY_EXPRESSION = "e";

		public static readonly string KEY_GROUP = "g";

		public static readonly string KEY_LIMIT = "l";

		public static readonly string KEY_FILTERED_ROOMS = "fr";

		private MatchExpression matchExpr;

		private string groupId;

		private int limit;

		public FindRoomsRequest(MatchExpression expr, string groupId, int limit)
			: base(RequestType.FindRooms)
		{
			Init(expr, groupId, limit);
		}

		public FindRoomsRequest(MatchExpression expr)
			: base(RequestType.FindRooms)
		{
			Init(expr, null, 0);
		}

		public FindRoomsRequest(MatchExpression expr, string groupId)
			: base(RequestType.FindRooms)
		{
			Init(expr, groupId, 0);
		}

		private void Init(MatchExpression expr, string groupId, int limit)
		{
			matchExpr = expr;
			this.groupId = groupId;
			this.limit = limit;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (matchExpr == null)
			{
				list.Add("Missing Match Expression");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("FindRooms request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutSFSArray(KEY_EXPRESSION, matchExpr.ToSFSArray());
			if (groupId != null)
			{
				sfso.PutUtfString(KEY_GROUP, groupId);
			}
			if (limit > 0)
			{
				sfso.PutShort(KEY_LIMIT, (short)limit);
			}
		}
	}
}
