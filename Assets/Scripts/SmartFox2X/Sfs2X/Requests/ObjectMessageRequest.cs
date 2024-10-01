using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	public class ObjectMessageRequest : GenericMessageRequest
	{
		public ObjectMessageRequest(ISFSObject obj, Room targetRoom, ICollection<User> recipients)
		{
			type = 4;
			parameters = obj;
			room = targetRoom;
			recipient = recipients;
		}

		public ObjectMessageRequest(ISFSObject obj, Room targetRoom)
			: this(obj, targetRoom, null)
		{
		}

		public ObjectMessageRequest(ISFSObject obj)
			: this(obj, null, null)
		{
		}
	}
}
