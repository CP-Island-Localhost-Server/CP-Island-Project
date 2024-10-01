using System;
using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using Sfs2X.Protocol;
using Sfs2X.Util;

namespace Sfs2X.Core
{
	public class WSProtocolCodec : IProtocolCodec
	{
		private static readonly string CONTROLLER_ID = "c";

		private static readonly string ACTION_ID = "a";

		private static readonly string PARAM_ID = "p";

		private IoHandler ioHandler = null;

		private ISocketClient socketClient;

		public IoHandler IOHandler
		{
			get
			{
				return ioHandler;
			}
			set
			{
				if (ioHandler != null)
				{
					throw new SFSError("IOHandler is already defined for this ProtocolHandler instance: " + this);
				}
				ioHandler = value;
			}
		}

		public WSProtocolCodec(IoHandler ioHandler, ISocketClient socketClient)
		{
			this.ioHandler = ioHandler;
			this.socketClient = socketClient;
		}

		public void OnPacketRead(ISFSObject packet)
		{
			DispatchRequest(packet);
		}

		public void OnPacketRead(ByteArray packet)
		{
		}

		public void OnPacketWrite(IMessage message)
		{
			ISFSObject content = PrepareWSPacket(message);
			message.Content = content;
			ioHandler.OnDataWrite(message);
		}

		private ISFSObject PrepareWSPacket(IMessage message)
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutByte(CONTROLLER_ID, Convert.ToByte(message.TargetController));
			iSFSObject.PutShort(ACTION_ID, Convert.ToInt16(message.Id));
			iSFSObject.PutSFSObject(PARAM_ID, message.Content);
			return iSFSObject;
		}

		private void DispatchRequest(ISFSObject requestObject)
		{
			IMessage message = new Message();
			if (requestObject.IsNull(CONTROLLER_ID))
			{
				throw new SFSCodecError("Request rejected: no Controller ID in request!");
			}
			if (requestObject.IsNull(ACTION_ID))
			{
				throw new SFSCodecError("Request rejected: no Action ID in request!");
			}
			message.Id = Convert.ToInt32(requestObject.GetShort(ACTION_ID));
			message.Content = requestObject.GetSFSObject(PARAM_ID);
			message.IsUDP = false;
			int @byte = requestObject.GetByte(CONTROLLER_ID);
			IController controller = socketClient.GetController(@byte);
			if (controller == null)
			{
				throw new SFSError("Cannot handle server response. Unknown controller, id: " + @byte);
			}
			controller.HandleMessage(message);
		}
	}
}
