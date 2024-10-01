using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Buddylist
{
	public class SetBuddyVariablesRequest : BaseRequest
	{
		public static readonly string KEY_BUDDY_NAME = "bn";

		public static readonly string KEY_BUDDY_VARS = "bv";

		private List<BuddyVariable> buddyVariables;

		public SetBuddyVariablesRequest(List<BuddyVariable> buddyVariables)
			: base(RequestType.SetBuddyVariables)
		{
			this.buddyVariables = buddyVariables;
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
				list.Add("Can't set buddy variables while off-line");
			}
			if (buddyVariables == null || buddyVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetBuddyVariables request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (BuddyVariable buddyVariable in buddyVariables)
			{
				iSFSArray.AddSFSArray(buddyVariable.ToSFSArray());
			}
			sfso.PutSFSArray(KEY_BUDDY_VARS, iSFSArray);
		}
	}
}
