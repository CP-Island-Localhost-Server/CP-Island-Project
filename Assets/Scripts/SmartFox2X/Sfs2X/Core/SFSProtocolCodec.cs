using System;
using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Protocol;
using Sfs2X.Util;

namespace Sfs2X.Core
{
	public class SFSProtocolCodec : IProtocolCodec
	{
		private static readonly string CONTROLLER_ID = "c";

		private static readonly string ACTION_ID = "a";

		private static readonly string PARAM_ID = "p";

		private static readonly string USER_ID = "u";

		private static readonly string UDP_PACKET_ID = "i";

		private IoHandler ioHandler = null;

		private Logger log;

		private ISocketClient bitSwarm;

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
					throw new SFSError("IOHandler is already defined for thir ProtocolHandler instance: " + this);
				}
				ioHandler = value;
			}
		}

		public SFSProtocolCodec(IoHandler ioHandler, ISocketClient bitSwarm)
		{
			this.ioHandler = ioHandler;
			log = bitSwarm.Log;
			this.bitSwarm = bitSwarm;
		}

		public void OnPacketRead(ByteArray packet)
		{
			ISFSObject requestObject = SFSObject.NewFromBinaryData(packet);
			DispatchRequest(requestObject);
		}

		public void OnPacketRead(ISFSObject packet)
		{
			DispatchRequest(packet);
		}

		public void OnPacketWrite(IMessage message)
		{
			if (bitSwarm.Debug)
			{
				log.Debug("Writing message " + message.Content.GetHexDump());
			}
			ISFSObject iSFSObject = null;
			iSFSObject = ((!message.IsUDP) ? PrepareTCPPacket(message) : PrepareUDPPacket(message));
			message.Content = iSFSObject;
			ioHandler.OnDataWrite(message);
		}

		private ISFSObject PrepareTCPPacket(IMessage message)
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutByte(CONTROLLER_ID, Convert.ToByte(message.TargetController));
			iSFSObject.PutShort(ACTION_ID, Convert.ToInt16(message.Id));
			iSFSObject.PutSFSObject(PARAM_ID, message.Content);
			return iSFSObject;
		}

		private ISFSObject PrepareUDPPacket(IMessage message)
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutByte(CONTROLLER_ID, Convert.ToByte(message.TargetController));
			iSFSObject.PutInt(USER_ID, (bitSwarm.Sfs.MySelf == null) ? (-1) : bitSwarm.Sfs.MySelf.Id);
			iSFSObject.PutLong(UDP_PACKET_ID, bitSwarm.NextUdpPacketId());
			iSFSObject.PutSFSObject(PARAM_ID, message.Content);
			return iSFSObject;
		}

		private void DispatchRequest(ISFSObject requestObject)
		{
			IMessage message = new Message();
			if (requestObject.IsNull(CONTROLLER_ID))
			{
				throw new SFSCodecError("Request rejected: No Controller ID in request!");
			}
			if (requestObject.IsNull(ACTION_ID))
			{
				throw new SFSCodecError("Request rejected: No Action ID in request!");
			}
			message.Id = Convert.ToInt32(requestObject.GetShort(ACTION_ID));
			message.Content = requestObject.GetSFSObject(PARAM_ID);
			message.IsUDP = requestObject.ContainsKey(UDP_PACKET_ID);
			if (message.IsUDP)
			{
				message.PacketId = requestObject.GetLong(UDP_PACKET_ID);
			}
			int @byte = requestObject.GetByte(CONTROLLER_ID);
			IController controller = bitSwarm.GetController(@byte);
			if (controller == null)
			{
				throw new SFSError("Cannot handle server response. Unknown controller, id: " + @byte);
			}
			controller.HandleMessage(message);
		}
	}
}
