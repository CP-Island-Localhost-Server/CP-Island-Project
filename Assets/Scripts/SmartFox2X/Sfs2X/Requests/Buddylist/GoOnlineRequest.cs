using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	public class GoOnlineRequest : BaseRequest
	{
		public static readonly string KEY_ONLINE = "o";

		public static readonly string KEY_BUDDY_NAME = "bn";

		public static readonly string KEY_BUDDY_ID = "bi";

		private bool online;

		public GoOnlineRequest(bool online)
			: base(RequestType.GoOnline)
		{
			this.online = online;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (!sfs.BuddyManager.Inited)
			{
				list.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("GoOnline request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfs.BuddyManager.MyOnlineState = online;
			sfso.PutBool(KEY_ONLINE, online);
		}
	}
}
