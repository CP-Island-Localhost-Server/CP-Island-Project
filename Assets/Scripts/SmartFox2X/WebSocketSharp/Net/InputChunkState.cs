namespace WebSocketSharp.Net
{
	internal enum InputChunkState
	{
		None = 0,
		Body = 1,
		BodyFinished = 2,
		Trailer = 3
	}
}
