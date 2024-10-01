using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities.Variables
{
	public abstract class BaseVariable : Variable
	{
		protected string name;

		protected VariableType type;

		protected object val;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public VariableType Type
		{
			get
			{
				return type;
			}
		}

		public object Value
		{
			get
			{
				return val;
			}
		}

		public BaseVariable(string name, object val, int type)
		{
			this.name = name;
			if (type > -1)
			{
				this.val = val;
				this.type = (VariableType)type;
			}
			else
			{
				SetValue(val);
			}
		}

		public BaseVariable(string name, object val)
			: this(name, val, -1)
		{
		}

		public bool GetBoolValue()
		{
			return (bool)val;
		}

		public int GetIntValue()
		{
			return (int)val;
		}

		public double GetDoubleValue()
		{
			return (double)val;
		}

		public string GetStringValue()
		{
			return val as string;
		}

		public ISFSObject GetSFSObjectValue()
		{
			return val as ISFSObject;
		}

		public ISFSArray GetSFSArrayValue()
		{
			return val as ISFSArray;
		}

		public bool IsNull()
		{
			return type == VariableType.NULL;
		}

		public virtual ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			iSFSArray.AddUtfString(name);
			iSFSArray.AddByte((byte)type);
			PopulateArrayWithValue(iSFSArray);
			return iSFSArray;
		}

		private void PopulateArrayWithValue(ISFSArray arr)
		{
			switch (type)
			{
			case VariableType.NULL:
				arr.AddNull();
				break;
			case VariableType.BOOL:
				arr.AddBool(GetBoolValue());
				break;
			case VariableType.INT:
				arr.AddInt(GetIntValue());
				break;
			case VariableType.DOUBLE:
				arr.AddDouble(GetDoubleValue());
				break;
			case VariableType.STRING:
				arr.AddUtfString(GetStringValue());
				break;
			case VariableType.OBJECT:
				arr.AddSFSObject(GetSFSObjectValue());
				break;
			case VariableType.ARRAY:
				arr.AddSFSArray(GetSFSArrayValue());
				break;
			}
		}

		private void SetValue(object val)
		{
			this.val = val;
			if (val == null)
			{
				type = VariableType.NULL;
			}
			else if (val is bool)
			{
				type = VariableType.BOOL;
			}
			else if (val is int)
			{
				type = VariableType.INT;
			}
			else if (val is double)
			{
				type = VariableType.DOUBLE;
			}
			else if (val is string)
			{
				type = VariableType.STRING;
			}
			else
			{
				if (!(val is object))
				{
					return;
				}
				string text = val.GetType().Name;
				if (text == "SFSObject")
				{
					type = VariableType.OBJECT;
					return;
				}
				if (!(text == "SFSArray"))
				{
					throw new SFSError("Unsupport SFS Variable type: " + text);
				}
				type = VariableType.ARRAY;
			}
		}
	}
}
