using System;
using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class GenericMessageRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";

		public static readonly string KEY_USER_ID = "u";

		public static readonly string KEY_MESSAGE = "m";

		public static readonly string KEY_MESSAGE_TYPE = "t";

		public static readonly string KEY_RECIPIENT = "rc";

		public static readonly string KEY_RECIPIENT_MODE = "rm";

		public static readonly string KEY_XTRA_PARAMS = "p";

		public static readonly string KEY_SENDER_DATA = "sd";

		protected int type = -1;

		protected Room room;

		protected User user;

		protected string message;

		protected ISFSObject parameters;

		protected object recipient;

		protected int sendMode;

		public GenericMessageRequest()
			: base(RequestType.GenericMessage)
		{
		}

		public override void Validate(SmartFox sfs)
		{
			if (type < 0)
			{
				throw new SFSValidationError("PublicMessage request error", new string[1] { "Unsupported message type: " + type });
			}
			List<string> list = new List<string>();
			switch ((GenericMessageType)type)
			{
			case GenericMessageType.PUBLIC_MSG:
				ValidatePublicMessage(sfs, list);
				break;
			case GenericMessageType.PRIVATE_MSG:
				ValidatePrivateMessage(sfs, list);
				break;
			case GenericMessageType.OBJECT_MSG:
				ValidateObjectMessage(sfs, list);
				break;
			case GenericMessageType.BUDDY_MSG:
				ValidateBuddyMessage(sfs, list);
				break;
			default:
				ValidateSuperUserMessage(sfs, list);
				break;
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("Request error - ", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutByte(KEY_MESSAGE_TYPE, Convert.ToByte(type));
			switch ((GenericMessageType)type)
			{
			case GenericMessageType.PUBLIC_MSG:
				ExecutePublicMessage(sfs);
				break;
			case GenericMessageType.PRIVATE_MSG:
				ExecutePrivateMessage(sfs);
				break;
			case GenericMessageType.OBJECT_MSG:
				ExecuteObjectMessage(sfs);
				break;
			case GenericMessageType.BUDDY_MSG:
				ExecuteBuddyMessage(sfs);
				break;
			default:
				ExecuteSuperUserMessage(sfs);
				break;
			}
		}

		private void ValidatePublicMessage(SmartFox sfs, List<string> errors)
		{
			if (message == null || message.Length == 0)
			{
				errors.Add("Public message is empty!");
			}
			if (room != null && !sfs.JoinedRooms.Contains(room))
			{
				errors.Add("You are not joined in the target Room: " + room);
			}
		}

		private void ValidatePrivateMessage(SmartFox sfs, List<string> errors)
		{
			if (message == null || message.Length == 0)
			{
				errors.Add("Private message is empty!");
			}
			if ((int)recipient < 0)
			{
				errors.Add("Invalid recipient id: " + recipient);
			}
		}

		private void ValidateObjectMessage(SmartFox sfs, List<string> errors)
		{
			if (parameters == null)
			{
				errors.Add("Object message is null!");
			}
		}

		private void ValidateBuddyMessage(SmartFox sfs, List<string> errors)
		{
			if (!sfs.BuddyManager.Inited)
			{
				errors.Add("BuddyList is not inited. Please send an InitBuddyRequest first.");
			}
			if (!sfs.BuddyManager.MyOnlineState)
			{
				errors.Add("Can't send messages while off-line");
			}
			if (message == null || message.Length == 0)
			{
				errors.Add("Buddy message is empty!");
			}
			int num = (int)recipient;
			if (num < 0)
			{
				errors.Add("Recipient is not online or not in your buddy list");
			}
		}

		private void ValidateSuperUserMessage(SmartFox sfs, List<string> errors)
		{
			if (message == null || message.Length == 0)
			{
				errors.Add("Moderator message is empty!");
			}
			switch ((MessageRecipientType)sendMode)
			{
			case MessageRecipientType.TO_USER:
				if (!(recipient is User))
				{
					errors.Add("TO_USER expects a User object as recipient");
				}
				break;
			case MessageRecipientType.TO_ROOM:
				if (!(recipient is Room))
				{
					errors.Add("TO_ROOM expects a Room object as recipient");
				}
				break;
			case MessageRecipientType.TO_GROUP:
				if (!(recipient is string))
				{
					errors.Add("TO_GROUP expects a String object (the groupId) as recipient");
				}
				break;
			}
		}

		private void ExecutePublicMessage(SmartFox sfs)
		{
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			if (room == null)
			{
				throw new SFSError("User should be joined in a room in order to send a public message");
			}
			sfso.PutInt(KEY_ROOM_ID, room.Id);
			sfso.PutInt(KEY_USER_ID, sfs.MySelf.Id);
			sfso.PutUtfString(KEY_MESSAGE, message);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_XTRA_PARAMS, parameters);
			}
		}

		private void ExecutePrivateMessage(SmartFox sfs)
		{
			sfso.PutInt(KEY_RECIPIENT, (int)recipient);
			sfso.PutUtfString(KEY_MESSAGE, message);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_XTRA_PARAMS, parameters);
			}
		}

		private void ExecuteBuddyMessage(SmartFox sfs)
		{
			sfso.PutInt(KEY_RECIPIENT, (int)recipient);
			sfso.PutUtfString(KEY_MESSAGE, message);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_XTRA_PARAMS, parameters);
			}
		}

		private void ExecuteSuperUserMessage(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_MESSAGE, message);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_XTRA_PARAMS, parameters);
			}
			sfso.PutInt(KEY_RECIPIENT_MODE, sendMode);
			switch ((MessageRecipientType)sendMode)
			{
			case MessageRecipientType.TO_USER:
				sfso.PutInt(KEY_RECIPIENT, ((User)recipient).Id);
				break;
			case MessageRecipientType.TO_ROOM:
				sfso.PutInt(KEY_RECIPIENT, ((Room)recipient).Id);
				break;
			case MessageRecipientType.TO_GROUP:
				sfso.PutUtfString(KEY_RECIPIENT, (string)recipient);
				break;
			}
		}

		private void ExecuteObjectMessage(SmartFox sfs)
		{
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			List<int> list = new List<int>();
			ICollection<User> collection = recipient as ICollection<User>;
			if (collection != null)
			{
				if (collection.Count > room.Capacity)
				{
					throw new ArgumentException("The number of recipients is bigger than the target Room capacity: " + collection.Count);
				}
				foreach (User item in collection)
				{
					if (!list.Contains(item.Id))
					{
						list.Add(item.Id);
					}
				}
			}
			sfso.PutInt(KEY_ROOM_ID, room.Id);
			sfso.PutSFSObject(KEY_XTRA_PARAMS, parameters);
			if (list.Count > 0)
			{
				sfso.PutIntArray(KEY_RECIPIENT, list.ToArray());
			}
		}
	}
}
