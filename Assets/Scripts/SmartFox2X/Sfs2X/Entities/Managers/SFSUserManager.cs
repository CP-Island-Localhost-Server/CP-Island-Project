using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public class SFSUserManager : IUserManager
	{
		private Dictionary<string, User> usersByName;

		private Dictionary<int, User> usersById;

		protected Room room;

		protected SmartFox sfs;

		public int UserCount
		{
			get
			{
				return usersById.Count;
			}
		}

		public SmartFox SmartFoxClient
		{
			get
			{
				return sfs;
			}
		}

		public SFSUserManager(SmartFox sfs)
		{
			this.sfs = sfs;
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}

		public SFSUserManager(Room room)
		{
			this.room = room;
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}

		protected void LogWarn(string msg)
		{
			if (sfs != null)
			{
				sfs.Log.Warn(msg);
			}
			else if (room != null && room.RoomManager != null)
			{
				room.RoomManager.SmartFoxClient.Log.Warn(msg);
			}
		}

		public bool ContainsUserName(string userName)
		{
			lock (usersByName)
			{
				return usersByName.ContainsKey(userName);
			}
		}

		public bool ContainsUserId(int userId)
		{
			lock (usersById)
			{
				return usersById.ContainsKey(userId);
			}
		}

		public bool ContainsUser(User user)
		{
			lock (usersByName)
			{
				return usersByName.ContainsValue(user);
			}
		}

		public User GetUserByName(string userName)
		{
			try
			{
				return usersByName[userName];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		public User GetUserById(int userId)
		{
			try
			{
				return usersById[userId];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		public virtual void AddUser(User user)
		{
			if (ContainsUserId(user.Id))
			{
				LogWarn("Unexpected: duplicate user in UserManager: " + user);
			}
			AddUserInternal(user);
		}

		protected void AddUserInternal(User user)
		{
			lock (usersByName)
			{
				usersByName[user.Name] = user;
			}
			lock (usersById)
			{
				usersById[user.Id] = user;
			}
		}

		public virtual void RemoveUser(User user)
		{
			lock (usersByName)
			{
				usersByName.Remove(user.Name);
			}
			lock (usersById)
			{
				usersById.Remove(user.Id);
			}
		}

		public void RemoveUserById(int id)
		{
			if (ContainsUserId(id))
			{
				User user = usersById[id];
				RemoveUser(user);
			}
		}

		public List<User> GetUserList()
		{
			lock (usersById)
			{
				return new List<User>(usersById.Values);
			}
		}

		public void ClearAll()
		{
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}
	}
}
