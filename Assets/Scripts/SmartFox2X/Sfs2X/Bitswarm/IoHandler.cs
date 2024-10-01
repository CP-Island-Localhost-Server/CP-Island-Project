using Sfs2X.Protocol;
using Sfs2X.Util;

namespace Sfs2X.Bitswarm
{
	public interface IoHandler
	{
		IProtocolCodec Codec { get; }

		void OnDataRead(ByteArray buffer);

		void OnDataRead(string jsonData);

		void OnDataWrite(IMessage message);
	}
}
