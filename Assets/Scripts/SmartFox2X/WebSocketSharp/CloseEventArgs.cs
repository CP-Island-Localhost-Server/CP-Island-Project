using System;
using System.Text;

namespace WebSocketSharp
{
	public class CloseEventArgs : EventArgs
	{
		private bool _clean;

		private ushort _code;

		private PayloadData _payloadData;

		private byte[] _rawData;

		private string _reason;

		internal PayloadData PayloadData
		{
			get
			{
				return _payloadData ?? (_payloadData = new PayloadData(_rawData));
			}
		}

		internal byte[] RawData
		{
			get
			{
				return _rawData;
			}
		}

		public ushort Code
		{
			get
			{
				return _code;
			}
		}

		public string Reason
		{
			get
			{
				return _reason;
			}
		}

		public bool WasClean
		{
			get
			{
				return _clean;
			}
			internal set
			{
				_clean = value;
			}
		}

		internal CloseEventArgs()
		{
			_payloadData = new PayloadData();
			_rawData = _payloadData.ApplicationData;
			_code = 1005;
			_reason = string.Empty;
		}

		internal CloseEventArgs(ushort code)
		{
			_code = code;
			_reason = string.Empty;
			_rawData = code.InternalToByteArray(ByteOrder.Big);
		}

		internal CloseEventArgs(CloseStatusCode code)
			: this((ushort)code)
		{
		}

		internal CloseEventArgs(PayloadData payloadData)
		{
			_payloadData = payloadData;
			_rawData = payloadData.ApplicationData;
			int num = _rawData.Length;
			_code = (ushort)((num <= 1) ? 1005 : _rawData.SubArray(0, 2).ToUInt16(ByteOrder.Big));
			_reason = ((num <= 2) ? string.Empty : Encoding.UTF8.GetString(_rawData.SubArray(2, num - 2)));
		}

		internal CloseEventArgs(ushort code, string reason)
		{
			_code = code;
			_reason = reason ?? string.Empty;
			_rawData = code.Append(reason);
		}

		internal CloseEventArgs(CloseStatusCode code, string reason)
			: this((ushort)code, reason)
		{
		}
	}
}
