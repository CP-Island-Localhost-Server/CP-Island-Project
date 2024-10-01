using System;

namespace Sfs2X.Core.Sockets
{
	public class WebSocketError
	{
		private Exception _exception;

		private string _message;

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public WebSocketError(string message)
			: this(message, null)
		{
		}

		public WebSocketError(string message, Exception exception)
		{
			_message = message;
			_exception = exception;
		}
	}
}
