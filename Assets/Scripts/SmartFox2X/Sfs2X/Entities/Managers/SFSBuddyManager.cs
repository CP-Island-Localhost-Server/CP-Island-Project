using System.Collections.Generic;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities.Managers
{
	public class SFSBuddyManager : IBuddyManager
	{
		protected Dictionary<string, Buddy> buddiesByName;

		protected Dictionary<string, BuddyVariable> myVariables;

		protected bool myOnlineState;

		protected bool inited;

		private List<string> buddyStates;

		public bool Inited
		{
			get
			{
				return inited;
			}
			set
			{
				inited = value;
			}
		}

		public List<Buddy> OfflineBuddies
		{
			get
			{
				List<Buddy> list = new List<Buddy>();
				lock (buddiesByName)
				{
					foreach (Buddy value in buddiesByName.Values)
					{
						if (!value.IsOnline)
						{
							list.Add(value);
						}
					}
					return list;
				}
			}
		}

		public List<Buddy> OnlineBuddies
		{
			get
			{
				List<Buddy> list = new List<Buddy>();
				lock (buddiesByName)
				{
					foreach (Buddy value in buddiesByName.Values)
					{
						if (value.IsOnline)
						{
							list.Add(value);
						}
					}
					return list;
				}
			}
		}

		public List<Buddy> BuddyList
		{
			get
			{
				lock (buddiesByName)
				{
					return new List<Buddy>(buddiesByName.Values);
				}
			}
		}

		public List<BuddyVariable> MyVariables
		{
			get
			{
				lock (myVariables)
				{
					return new List<BuddyVariable>(myVariables.Values);
				}
			}
			set
			{
				foreach (BuddyVariable item in value)
				{
					SetMyVariable(item);
				}
			}
		}

		public bool MyOnlineState
		{
			get
			{
				if (!inited)
				{
					return false;
				}
				bool result = true;
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_ONLINE);
				if (myVariable != null)
				{
					result = myVariable.GetBoolValue();
				}
				return result;
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_ONLINE, value));
			}
		}

		public string MyNickName
		{
			get
			{
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_NICKNAME);
				return (myVariable == null) ? null : myVariable.GetStringValue();
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_NICKNAME, value));
			}
		}

		public string MyState
		{
			get
			{
				BuddyVariable myVariable = GetMyVariable(ReservedBuddyVariables.BV_STATE);
				return (myVariable == null) ? null : myVariable.GetStringValue();
			}
			set
			{
				SetMyVariable(new SFSBuddyVariable(ReservedBuddyVariables.BV_STATE, value));
			}
		}

		public List<string> BuddyStates
		{
			get
			{
				return buddyStates;
			}
			set
			{
				buddyStates = value;
			}
		}

		public SFSBuddyManager(SmartFox sfs)
		{
			buddiesByName = new Dictionary<string, Buddy>();
			myVariables = new Dictionary<string, BuddyVariable>();
			inited = false;
		}

		public void AddBuddy(Buddy buddy)
		{
			lock (buddiesByName)
			{
				buddiesByName.Add(buddy.Name, buddy);
			}
		}

		public void ClearAll()
		{
			lock (buddiesByName)
			{
				buddiesByName.Clear();
			}
		}

		public Buddy RemoveBuddyById(int id)
		{
			Buddy buddyById = GetBuddyById(id);
			if (buddyById != null)
			{
				lock (buddiesByName)
				{
					buddiesByName.Remove(buddyById.Name);
				}
			}
			return buddyById;
		}

		public Buddy RemoveBuddyByName(string name)
		{
			Buddy buddyByName = GetBuddyByName(name);
			if (buddyByName != null)
			{
				lock (buddiesByName)
				{
					buddiesByName.Remove(name);
				}
			}
			return buddyByName;
		}

		public Buddy GetBuddyById(int id)
		{
			if (id > -1)
			{
				lock (buddiesByName)
				{
					foreach (Buddy value in buddiesByName.Values)
					{
						if (value.Id == id)
						{
							return value;
						}
					}
				}
			}
			return null;
		}

		public bool ContainsBuddy(string name)
		{
			lock (buddiesByName)
			{
				return buddiesByName.ContainsKey(name);
			}
		}

		public Buddy GetBuddyByName(string name)
		{
			try
			{
				return buddiesByName[name];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		public Buddy GetBuddyByNickName(string nickName)
		{
			lock (buddiesByName)
			{
				foreach (Buddy value in buddiesByName.Values)
				{
					if (value.NickName == nickName)
					{
						return value;
					}
				}
			}
			return null;
		}

		public BuddyVariable GetMyVariable(string varName)
		{
			try
			{
				return myVariables[varName];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		public void SetMyVariable(BuddyVariable bVar)
		{
			lock (myVariables)
			{
				myVariables[bVar.Name] = bVar;
			}
		}
	}
}
