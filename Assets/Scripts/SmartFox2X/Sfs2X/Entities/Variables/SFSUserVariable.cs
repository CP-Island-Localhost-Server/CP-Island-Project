using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	public class SFSUserVariable : BaseVariable, UserVariable, Variable
	{
		private bool isPrivate;

		public bool IsPrivate
		{
			get
			{
				return isPrivate;
			}
			set
			{
				isPrivate = value;
			}
		}

		public SFSUserVariable(string name, object val, int type)
			: base(name, val, type)
		{
			isPrivate = false;
		}

		public SFSUserVariable(string name, object val)
			: base(name, val)
		{
			isPrivate = false;
		}

		public static UserVariable FromSFSArray(ISFSArray sfsa)
		{
			UserVariable userVariable = new SFSUserVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
			if (sfsa.Count > 3)
			{
				userVariable.IsPrivate = sfsa.GetBool(3);
			}
			return userVariable;
		}

		public static SFSUserVariable newPrivateVariable(string name, object val)
		{
			SFSUserVariable sFSUserVariable = new SFSUserVariable(name, val);
			sFSUserVariable.IsPrivate = true;
			return sFSUserVariable;
		}

		public override ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = base.ToSFSArray();
			iSFSArray.AddBool(isPrivate);
			return iSFSArray;
		}

		public override string ToString()
		{
			return string.Concat("[UserVar: ", name, ", type: ", type, ", value: ", val, ", private: ", isPrivate, "]");
		}
	}
}
