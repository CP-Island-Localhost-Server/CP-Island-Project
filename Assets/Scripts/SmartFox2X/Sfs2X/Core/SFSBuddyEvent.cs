using System.Collections;

namespace Sfs2X.Core
{
	public class SFSBuddyEvent : BaseEvent
	{
		public static readonly string BUDDY_LIST_INIT = "buddyListInit";

		public static readonly string BUDDY_ADD = "buddyAdd";

		public static readonly string BUDDY_REMOVE = "buddyRemove";

		public static readonly string BUDDY_BLOCK = "buddyBlock";

		public static readonly string BUDDY_ERROR = "buddyError";

		public static readonly string BUDDY_ONLINE_STATE_UPDATE = "buddyOnlineStateChange";

		public static readonly string BUDDY_VARIABLES_UPDATE = "buddyVariablesUpdate";

		public static readonly string BUDDY_MESSAGE = "buddyMessage";

		public SFSBuddyEvent(string type)
			: base(type, null)
		{
		}

		public SFSBuddyEvent(string type, Hashtable args)
			: base(type, args)
		{
		}
	}
}
