namespace Sfs2X.Bitswarm
{
	public enum PacketReadTransition
	{
		HeaderReceived = 0,
		SizeReceived = 1,
		IncompleteSize = 2,
		WholeSizeReceived = 3,
		PacketFinished = 4,
		InvalidData = 5,
		InvalidDataFinished = 6
	}
}
