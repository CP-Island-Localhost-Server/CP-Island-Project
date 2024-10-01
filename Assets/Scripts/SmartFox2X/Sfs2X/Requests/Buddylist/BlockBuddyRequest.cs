using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	public class BlockBuddyRequest : BaseRequest
	{
		public static readonly string KEY_BUDDY_NAME = "bn";

		public static readonly string KEY_BUDDY_BLOCK_STATE = "bs";

		public static readonly string KEY_BUDDY = "bd";

		private string buddyName;

		private bool blocked;

		public BlockBuddyRequest(string buddyName, bool blocked)
			: base(RequestType.BlockBuddy)
		{
			this.buddyName = buddyName;
			this.blocked = blocked;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (buddyName == null || buddyName.Length < 1)
			{
				list.Add("Invalid buddy name: " + buddyName);
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				list.Add("Can't block buddy while off-line");
			}
			Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(buddyName);
			if (buddyByName == null)
			{
				list.Add("Can't block buddy, it's not in your list: " + buddyName);
			}
			else if (buddyByName.IsBlocked == blocked)
			{
				list.Add("BuddyBlock flag is already in the requested state: " + blocked + ", for buddy: " + buddyByName);
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("BuddyList request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_BUDDY_NAME, buddyName);
			sfso.PutBool(KEY_BUDDY_BLOCK_STATE, blocked);
		}
	}
}
