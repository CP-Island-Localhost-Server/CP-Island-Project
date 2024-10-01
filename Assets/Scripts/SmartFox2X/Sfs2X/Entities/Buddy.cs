using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public interface Buddy
	{
		int Id { get; set; }

		string Name { get; }

		bool IsBlocked { get; set; }

		bool IsOnline { get; }

		bool IsTemp { get; }

		string State { get; }

		string NickName { get; }

		List<BuddyVariable> Variables { get; }

		BuddyVariable GetVariable(string varName);

		bool ContainsVariable(string varName);

		List<BuddyVariable> GetOfflineVariables();

		List<BuddyVariable> GetOnlineVariables();

		void SetVariable(BuddyVariable bVar);

		void SetVariables(ICollection<BuddyVariable> variables);

		void RemoveVariable(string varName);

		void ClearVolatileVariables();
	}
}
