using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	public class RemoveBuddyRequest : BaseRequest
	{
		public static readonly string KEY_BUDDY_NAME = "bn";

		private string name;

		public RemoveBuddyRequest(string buddyName)
			: base(RequestType.RemoveBuddy)
		{
			name = buddyName;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't remove buddy while off-line");
			}
			if (!sfs.BuddyManager.ContainsBuddy(name))
			{
				list.Add("Can't remove buddy, it's not in your list: " + name);
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BuddyList request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_BUDDY_NAME, name);
		}
	}
}
