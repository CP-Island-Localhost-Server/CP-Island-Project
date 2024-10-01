using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	public class SFSBuddyVariable : BaseVariable, BuddyVariable, Variable
	{
		public static readonly string OFFLINE_PREFIX = "$";

		public bool IsOffline
		{
			get
			{
				return name.StartsWith("$");
			}
		}

		public SFSBuddyVariable(string name, object val, int type)
			: base(name, val, type)
		{
		}

		public SFSBuddyVariable(string name, object val)
			: base(name, val)
		{
		}

		public static BuddyVariable FromSFSArray(ISFSArray sfsa)
		{
			return new SFSBuddyVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
		}

		public override string ToString()
		{
			return string.Concat("[BuddyVar: ", name, ", type: ", type, ", value: ", val, "]");
		}
	}
}
