using System.Text;
using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Protocol;
using Sfs2X.Util;

namespace Sfs2X.Core
{
	public class WSIOHandler : IoHandler
	{
		private ISocketClient socketClient;

		private Logger log;

		private IProtocolCodec protocolCodec;

		public IProtocolCodec Codec
		{
			get
			{
				return protocolCodec;
			}
		}

		public WSIOHandler(ISocketClient socketClient)
		{
			this.socketClient = socketClient;
			log = socketClient.Log;
			protocolCodec = new WSProtocolCodec(this, socketClient);
		}

		public void OnDataWrite(IMessage message)
		{
			string text = message.Content.ToJson();
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			if (bytes.Length > socketClient.MaxMessageSize)
			{
				throw new SFSCodecError("Message size is too big: " + bytes.Length + ", the server limit is: " + socketClient.MaxMessageSize);
			}
			if (socketClient.Debug)
			{
				log.Info("Data written [" + bytes.Length + " bytes]: " + text);
			}
			socketClient.Socket.Write(text);
		}

		public void OnDataRead(string jsonData)
		{
			if (jsonData.Length == 0)
			{
				throw new SFSError("Unexpected empty string data: no readable informations available!");
			}
			if (socketClient.Debug)
			{
				log.Info("Data read: " + jsonData);
			}
			ISFSObject packet = SFSObject.NewFromJsonData(jsonData);
			protocolCodec.OnPacketRead(packet);
		}

		public void OnDataRead(ByteArray data)
		{
		}
	}
}
