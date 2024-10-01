namespace Sfs2X.Bitswarm
{
	public enum PacketReadState
	{
		WAIT_NEW_PACKET = 0,
		WAIT_DATA_SIZE = 1,
		WAIT_DATA_SIZE_FRAGMENT = 2,
		WAIT_DATA = 3,
		INVALID_DATA = 4
	}
}
