using System;

namespace WebSocketSharp
{
	public class ErrorEventArgs : EventArgs
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

		internal ErrorEventArgs(string message)
			: this(message, null)
		{
		}

		internal ErrorEventArgs(string message, Exception exception)
		{
			_message = message;
			_exception = exception;
		}
	}
}
