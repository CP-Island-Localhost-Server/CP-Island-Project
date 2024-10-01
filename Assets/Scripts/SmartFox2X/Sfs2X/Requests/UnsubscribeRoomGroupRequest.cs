using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class UnsubscribeRoomGroupRequest : BaseRequest
	{
		public static readonly string KEY_GROUP_ID = "g";

		private string groupId;

		public UnsubscribeRoomGroupRequest(string groupId)
			: base(RequestType.UnsubscribeRoomGroup)
		{
			this.groupId = groupId;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (groupId == null || groupId.Length == 0)
			{
				list.Add("Invalid groupId. Must be a string with at least 1 character.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("UnsubscribeGroup request Error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_GROUP_ID, groupId);
		}
	}
}
