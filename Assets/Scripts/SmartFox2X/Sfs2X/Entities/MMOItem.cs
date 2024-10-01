using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public class MMOItem : IMMOItem
	{
		private int id;

		private Vec3D aoiEntryPoint;

		private Dictionary<string, IMMOItemVariable> variables = new Dictionary<string, IMMOItemVariable>();

		public int Id
		{
			get
			{
				return id;
			}
		}

		public Vec3D AOIEntryPoint
		{
			get
			{
				return aoiEntryPoint;
			}
			set
			{
				aoiEntryPoint = value;
			}
		}

		public MMOItem(int id)
		{
			this.id = id;
		}

		public static IMMOItem FromSFSArray(ISFSArray encodedItem)
		{
			IMMOItem iMMOItem = new MMOItem(encodedItem.GetInt(0));
			ISFSArray sFSArray = encodedItem.GetSFSArray(1);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				iMMOItem.SetVariable(MMOItemVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return iMMOItem;
		}

		public List<IMMOItemVariable> GetVariables()
		{
			return new List<IMMOItemVariable>(variables.Values);
		}

		public IMMOItemVariable GetVariable(string name)
		{
			return variables[name];
		}

		public void SetVariable(IMMOItemVariable variable)
		{
			if (variable.IsNull())
			{
				variables.Remove(variable.Name);
			}
			else
			{
				variables[variable.Name] = variable;
			}
		}

		public void SetVariables(List<IMMOItemVariable> variables)
		{
			foreach (IMMOItemVariable variable in variables)
			{
				SetVariable(variable);
			}
		}

		public bool ContainsVariable(string name)
		{
			return variables.ContainsKey(name);
		}
	}
}
