using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public class SFSGlobalUserManager : SFSUserManager, IUserManager
	{
		private Dictionary<User, int> roomRefCount;

		public SFSGlobalUserManager(SmartFox sfs)
			: base(sfs)
		{
			roomRefCount = new Dictionary<User, int>();
		}

		public SFSGlobalUserManager(Room room)
			: base(room)
		{
			roomRefCount = new Dictionary<User, int>();
		}

		public override void AddUser(User user)
		{
			lock (roomRefCount)
			{
				if (!roomRefCount.ContainsKey(user))
				{
					base.AddUser(user);
					roomRefCount[user] = 1;
					return;
				}
				Dictionary<User, int> dictionary;
				Dictionary<User, int> dictionary2 = (dictionary = roomRefCount);
				User key;
				User key2 = (key = user);
				int num = dictionary[key];
				dictionary2[key2] = num + 1;
			}
		}

		public override void RemoveUser(User user)
		{
			RemoveUserReference(user, false);
		}

		public void RemoveUserReference(User user, bool disconnected)
		{
			lock (roomRefCount)
			{
				if (roomRefCount.ContainsKey(user))
				{
					if (roomRefCount[user] < 1)
					{
						LogWarn("GlobalUserManager RefCount is already at zero. User: " + user);
						return;
					}
					Dictionary<User, int> dictionary;
					Dictionary<User, int> dictionary2 = (dictionary = roomRefCount);
					User key;
					User key2 = (key = user);
					int num = dictionary[key];
					dictionary2[key2] = num - 1;
					if (roomRefCount[user] == 0 || disconnected)
					{
						base.RemoveUser(user);
						roomRefCount.Remove(user);
					}
				}
				else
				{
					LogWarn("Can't remove User from GlobalUserManager. RefCount missing. User: " + user);
				}
			}
		}
	}
}
