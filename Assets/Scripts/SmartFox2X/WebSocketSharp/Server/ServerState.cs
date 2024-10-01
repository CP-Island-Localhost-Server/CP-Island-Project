namespace WebSocketSharp.Server
{
	internal enum ServerState
	{
		Ready = 0,
		Start = 1,
		ShuttingDown = 2,
		Stop = 3
	}
}
