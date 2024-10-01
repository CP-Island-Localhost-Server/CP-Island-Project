using System;

namespace WebSocketSharp
{
	public class WebSocketException : Exception
	{
		private CloseStatusCode _code;

		public CloseStatusCode Code
		{
			get
			{
				return _code;
			}
		}

		internal WebSocketException()
			: this(CloseStatusCode.Abnormal, null, null)
		{
		}

		internal WebSocketException(Exception innerException)
			: this(CloseStatusCode.Abnormal, null, innerException)
		{
		}

		internal WebSocketException(string message)
			: this(CloseStatusCode.Abnormal, message, null)
		{
		}

		internal WebSocketException(CloseStatusCode code)
			: this(code, null, null)
		{
		}

		internal WebSocketException(string message, Exception innerException)
			: this(CloseStatusCode.Abnormal, message, innerException)
		{
		}

		internal WebSocketException(CloseStatusCode code, Exception innerException)
			: this(code, null, innerException)
		{
		}

		internal WebSocketException(CloseStatusCode code, string message)
			: this(code, message, null)
		{
		}

		internal WebSocketException(CloseStatusCode code, string message, Exception innerException)
			: base(message ?? code.GetMessage(), innerException)
		{
			_code = code;
		}
	}
}
