using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	public class MMOItemVariable : BaseVariable, IMMOItemVariable, Variable
	{
		public MMOItemVariable(string name, object val, int type)
			: base(name, val, type)
		{
		}

		public MMOItemVariable(string name, object val)
			: base(name, val)
		{
		}

		public static IMMOItemVariable FromSFSArray(ISFSArray sfsa)
		{
			return new MMOItemVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
		}

		public override string ToString()
		{
			return string.Concat("[MMOItemVar: ", name, ", type: ", type, ", value: ", val, "]");
		}
	}
}
