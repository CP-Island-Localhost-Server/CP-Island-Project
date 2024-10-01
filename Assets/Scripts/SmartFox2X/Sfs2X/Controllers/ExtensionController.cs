using System.Collections;
using Sfs2X.Bitswarm;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

namespace Sfs2X.Controllers
{
	public class ExtensionController : BaseController
	{
		public static readonly string KEY_CMD = "c";

		public static readonly string KEY_PARAMS = "p";

		public static readonly string KEY_ROOM = "r";

		public ExtensionController(ISocketClient socketClient)
			: base(socketClient)
		{
		}

		public override void HandleMessage(IMessage message)
		{
			if (sfs.Debug)
			{
				log.Info(message.ToString());
			}
			ISFSObject content = message.Content;
			Hashtable hashtable = new Hashtable();
			hashtable["cmd"] = content.GetUtfString(KEY_CMD);
			hashtable["params"] = content.GetSFSObject(KEY_PARAMS);
			if (content.ContainsKey(KEY_ROOM))
			{
				int @int = content.GetInt(KEY_ROOM);
				hashtable["sourceRoom"] = @int;
				hashtable["room"] = sfs.GetRoomById(@int);
			}
			if (message.IsUDP)
			{
				hashtable["packetId"] = message.PacketId;
			}
			sfs.DispatchEvent(new SFSEvent(SFSEvent.EXTENSION_RESPONSE, hashtable));
		}
	}
}
