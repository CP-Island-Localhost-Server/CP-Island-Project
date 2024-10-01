using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities
{
	public class SFSUser : User
	{
		protected int id = -1;

		protected int privilegeId = 0;

		protected string name;

		protected bool isItMe;

		protected Dictionary<string, UserVariable> variables;

		protected Dictionary<string, object> properties;

		protected bool isModerator;

		protected Dictionary<int, int> playerIdByRoomId;

		protected IUserManager userManager;

		protected Vec3D aoiEntryPoint;

		public int Id
		{
			get
			{
				return id;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int PlayerId
		{
			get
			{
				return GetPlayerId(userManager.SmartFoxClient.LastJoinedRoom);
			}
		}

		public int PrivilegeId
		{
			get
			{
				return privilegeId;
			}
			set
			{
				privilegeId = value;
			}
		}

		public bool IsPlayer
		{
			get
			{
				return PlayerId > 0;
			}
		}

		public bool IsSpectator
		{
			get
			{
				return !IsPlayer;
			}
		}

		public bool IsItMe
		{
			get
			{
				return isItMe;
			}
		}

		public IUserManager UserManager
		{
			get
			{
				return userManager;
			}
			set
			{
				if (userManager != null)
				{
					throw new SFSError("Cannot re-assign the User manager. Already set. User: " + this);
				}
				userManager = value;
			}
		}

		public Dictionary<string, object> Properties
		{
			get
			{
				return properties;
			}
			set
			{
				properties = value;
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

		public SFSUser(int id, string name)
		{
			Init(id, name, false);
		}

		public SFSUser(int id, string name, bool isItMe)
		{
			Init(id, name, isItMe);
		}

		public static User FromSFSArray(ISFSArray sfsa, Room room)
		{
			User user = new SFSUser(sfsa.GetInt(0), sfsa.GetUtfString(1));
			user.PrivilegeId = sfsa.GetShort(2);
			if (room != null)
			{
				user.SetPlayerId(sfsa.GetShort(3), room);
			}
			ISFSArray sFSArray = sfsa.GetSFSArray(4);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				user.SetVariable(SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return user;
		}

		public static User FromSFSArray(ISFSArray sfsa)
		{
			return FromSFSArray(sfsa, null);
		}

		private void Init(int id, string name, bool isItMe)
		{
			this.id = id;
			this.name = name;
			this.isItMe = isItMe;
			variables = new Dictionary<string, UserVariable>();
			properties = new Dictionary<string, object>();
			isModerator = false;
			playerIdByRoomId = new Dictionary<int, int>();
		}

		public bool IsJoinedInRoom(Room room)
		{
			return room.ContainsUser(this);
		}

		public bool IsGuest()
		{
			return privilegeId == 0;
		}

		public bool IsStandardUser()
		{
			return privilegeId == 1;
		}

		public bool IsModerator()
		{
			return privilegeId == 2;
		}

		public bool IsAdmin()
		{
			return privilegeId == 3;
		}

		public int GetPlayerId(Room room)
		{
			int result = 0;
			if (playerIdByRoomId.ContainsKey(room.Id))
			{
				result = playerIdByRoomId[room.Id];
			}
			return result;
		}

		public void SetPlayerId(int id, Room room)
		{
			playerIdByRoomId[room.Id] = id;
		}

		public void RemovePlayerId(Room room)
		{
			playerIdByRoomId.Remove(room.Id);
		}

		public bool IsPlayerInRoom(Room room)
		{
			return playerIdByRoomId[room.Id] > 0;
		}

		public bool IsSpectatorInRoom(Room room)
		{
			return playerIdByRoomId[room.Id] < 0;
		}

		public List<UserVariable> GetVariables()
		{
			return new List<UserVariable>(variables.Values);
		}

		public UserVariable GetVariable(string name)
		{
			if (!variables.ContainsKey(name))
			{
				return null;
			}
			return variables[name];
		}

		public void SetVariable(UserVariable userVariable)
		{
			if (userVariable != null)
			{
				if (userVariable.IsNull())
				{
					variables.Remove(userVariable.Name);
				}
				else
				{
					variables[userVariable.Name] = userVariable;
				}
			}
		}

		public void SetVariables(ICollection<UserVariable> userVariables)
		{
			foreach (UserVariable userVariable in userVariables)
			{
				SetVariable(userVariable);
			}
		}

		public bool ContainsVariable(string name)
		{
			return variables.ContainsKey(name);
		}

		private void RemoveUserVariable(string varName)
		{
			variables.Remove(varName);
		}

		public override string ToString()
		{
			return "[User: " + name + ", Id: " + id + ", isMe: " + isItMe + "]";
		}
	}
}
