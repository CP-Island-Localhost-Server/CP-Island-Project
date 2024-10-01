using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public interface IMMOItem
	{
		int Id { get; }

		Vec3D AOIEntryPoint { get; set; }

		List<IMMOItemVariable> GetVariables();

		IMMOItemVariable GetVariable(string name);

		void SetVariable(IMMOItemVariable variable);

		void SetVariables(List<IMMOItemVariable> variables);

		bool ContainsVariable(string name);
	}
}
