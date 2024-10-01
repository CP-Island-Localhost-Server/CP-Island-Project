using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public interface IRoomManager
	{
		string OwnerZone { get; }

		SmartFox SmartFoxClient { get; }

		void AddRoom(Room room, bool addGroupIfMissing);

		void AddRoom(Room room);

		void AddGroup(string groupId);

		Room ReplaceRoom(Room room, bool addToGroupIfMissing);

		Room ReplaceRoom(Room room);

		void RemoveGroup(string groupId);

		bool ContainsGroup(string groupId);

		bool ContainsRoom(object idOrName);

		bool ContainsRoomInGroup(object idOrName, string groupId);

		void ChangeRoomName(Room room, string newName);

		void ChangeRoomPasswordState(Room room, bool isPassProtected);

		void ChangeRoomCapacity(Room room, int maxUsers, int maxSpect);

		Room GetRoomById(int id);

		Room GetRoomByName(string name);

		List<Room> GetRoomList();

		int GetRoomCount();

		List<string> GetRoomGroups();

		List<Room> GetRoomListFromGroup(string groupId);

		List<Room> GetJoinedRooms();

		List<Room> GetUserRooms(User user);

		void RemoveRoom(Room room);

		void RemoveRoomById(int id);

		void RemoveRoomByName(string name);

		void RemoveUser(User user);
	}
}
