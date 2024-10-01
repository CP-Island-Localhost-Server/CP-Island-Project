namespace WebSocketSharp
{
	public enum WebSocketState : ushort
	{
		Connecting = 0,
		Open = 1,
		Closing = 2,
		Closed = 3
	}
}
