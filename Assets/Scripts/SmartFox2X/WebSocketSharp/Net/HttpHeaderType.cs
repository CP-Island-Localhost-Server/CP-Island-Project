using System;

namespace WebSocketSharp.Net
{
	[Flags]
	internal enum HttpHeaderType
	{
		Unspecified = 0,
		Request = 1,
		Response = 2,
		Restricted = 4,
		MultiValue = 8,
		MultiValueInRequest = 0x10,
		MultiValueInResponse = 0x20
	}
}
