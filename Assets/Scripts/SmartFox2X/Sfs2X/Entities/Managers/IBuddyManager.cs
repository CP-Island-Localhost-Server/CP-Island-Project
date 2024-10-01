using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities.Managers
{
	public interface IBuddyManager
	{
		bool Inited { get; set; }

		List<Buddy> OfflineBuddies { get; }

		List<Buddy> OnlineBuddies { get; }

		List<Buddy> BuddyList { get; }

		List<string> BuddyStates { get; set; }

		List<BuddyVariable> MyVariables { get; set; }

		bool MyOnlineState { get; set; }

		string MyNickName { get; set; }

		string MyState { get; set; }

		void AddBuddy(Buddy buddy);

		Buddy RemoveBuddyById(int id);

		Buddy RemoveBuddyByName(string name);

		bool ContainsBuddy(string name);

		Buddy GetBuddyById(int id);

		Buddy GetBuddyByName(string name);

		Buddy GetBuddyByNickName(string nickName);

		BuddyVariable GetMyVariable(string varName);

		void SetMyVariable(BuddyVariable bVar);

		void ClearAll();
	}
}
