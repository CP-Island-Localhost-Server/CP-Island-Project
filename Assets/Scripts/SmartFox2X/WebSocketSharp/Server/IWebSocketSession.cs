using System;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public interface IWebSocketSession
	{
		WebSocketContext Context { get; }

		string ID { get; }

		string Protocol { get; }

		DateTime StartTime { get; }

		WebSocketState State { get; }
	}
}
