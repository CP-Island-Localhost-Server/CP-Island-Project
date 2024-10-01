using System;
using System.Collections;
using System.Collections.Generic;

namespace WebSocketSharp
{
	internal class PayloadData : IEnumerable, IEnumerable<byte>
	{
		public const ulong MaxLength = 9223372036854775807uL;

		private byte[] _data;

		private long _extDataLength;

		private long _length;

		private bool _masked;

		internal long ExtensionDataLength
		{
			get
			{
				return _extDataLength;
			}
			set
			{
				_extDataLength = value;
			}
		}

		internal bool IncludesReservedCloseStatusCode
		{
			get
			{
				return _length > 1 && _data.SubArray(0, 2).ToUInt16(ByteOrder.Big).IsReserved();
			}
		}

		public byte[] ApplicationData
		{
			get
			{
				return (_extDataLength <= 0) ? _data : _data.SubArray(_extDataLength, _length - _extDataLength);
			}
		}

		public byte[] ExtensionData
		{
			get
			{
				return (_extDataLength <= 0) ? new byte[0] : _data.SubArray(0L, _extDataLength);
			}
		}

		public bool IsMasked
		{
			get
			{
				return _masked;
			}
		}

		public ulong Length
		{
			get
			{
				return (ulong)_length;
			}
		}

		internal PayloadData()
		{
			_data = new byte[0];
		}

		internal PayloadData(byte[] data)
			: this(data, false)
		{
		}

		internal PayloadData(byte[] data, bool masked)
		{
			_data = data;
			_masked = masked;
			_length = data.LongLength;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void Mask(byte[] key)
		{
			for (long num = 0L; num < _length; num++)
			{
				_data[num] ^= key[num % 4];
			}
			_masked = !_masked;
		}

		public IEnumerator<byte> GetEnumerator()
		{
			byte[] data = _data;
			for (int i = 0; i < data.Length; i++)
			{
				yield return data[i];
			}
		}

		public byte[] ToByteArray()
		{
			return _data;
		}

		public override string ToString()
		{
			return BitConverter.ToString(_data);
		}
	}
}
