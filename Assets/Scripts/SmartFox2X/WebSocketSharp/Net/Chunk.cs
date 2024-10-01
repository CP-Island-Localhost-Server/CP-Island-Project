using System;

namespace WebSocketSharp.Net
{
	internal class Chunk
	{
		private byte[] _data;

		private int _offset;

		public int ReadLeft
		{
			get
			{
				return _data.Length - _offset;
			}
		}

		public Chunk(byte[] data)
		{
			_data = data;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = _data.Length - _offset;
			if (num == 0)
			{
				return num;
			}
			if (count > num)
			{
				count = num;
			}
			Buffer.BlockCopy(_data, _offset, buffer, offset, count);
			_offset += count;
			return count;
		}
	}
}
