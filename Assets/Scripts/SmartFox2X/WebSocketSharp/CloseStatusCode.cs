namespace WebSocketSharp
{
	public enum CloseStatusCode : ushort
	{
		Normal = 1000,
		Away = 1001,
		ProtocolError = 1002,
		IncorrectData = 1003,
		Undefined = 1004,
		NoStatusCode = 1005,
		Abnormal = 1006,
		InconsistentData = 1007,
		PolicyViolation = 1008,
		TooBig = 1009,
		IgnoreExtension = 1010,
		ServerError = 1011,
		TlsHandshakeFailure = 1015
	}
}
