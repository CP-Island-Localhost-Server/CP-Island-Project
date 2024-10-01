using System;
using System.Text;

namespace WebSocketSharp
{
	public class MessageEventArgs : EventArgs
	{
		private string _data;

		private Opcode _opcode;

		private byte[] _rawData;

		public string Data
		{
			get
			{
				return _data;
			}
		}

		public byte[] RawData
		{
			get
			{
				return _rawData;
			}
		}

		public Opcode Type
		{
			get
			{
				return _opcode;
			}
		}

		internal MessageEventArgs(WebSocketFrame frame)
		{
			_opcode = frame.Opcode;
			_rawData = frame.PayloadData.ApplicationData;
			_data = convertToString(_opcode, _rawData);
		}

		internal MessageEventArgs(Opcode opcode, byte[] rawData)
		{
			if ((ulong)rawData.LongLength > 9223372036854775807uL)
			{
				throw new WebSocketException(CloseStatusCode.TooBig);
			}
			_opcode = opcode;
			_rawData = rawData;
			_data = convertToString(opcode, rawData);
		}

		private static string convertToString(Opcode opcode, byte[] rawData)
		{
			return (rawData.LongLength == 0) ? string.Empty : ((opcode != Opcode.Text) ? opcode.ToString() : Encoding.UTF8.GetString(rawData));
		}
	}
}
