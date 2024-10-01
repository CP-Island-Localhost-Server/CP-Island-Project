using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class ChunkStream
	{
		private int _chunkRead;

		private int _chunkSize;

		private List<Chunk> _chunks;

		private bool _gotIt;

		private WebHeaderCollection _headers;

		private StringBuilder _saved;

		private bool _sawCr;

		private InputChunkState _state;

		private int _trailerState;

		internal WebHeaderCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		public int ChunkLeft
		{
			get
			{
				return _chunkSize - _chunkRead;
			}
		}

		public bool WantMore
		{
			get
			{
				return _chunkRead != _chunkSize || _chunkSize != 0 || _state != InputChunkState.None;
			}
		}

		public ChunkStream(WebHeaderCollection headers)
		{
			_headers = headers;
			_chunkSize = -1;
			_chunks = new List<Chunk>();
			_saved = new StringBuilder();
		}

		public ChunkStream(byte[] buffer, int offset, int count, WebHeaderCollection headers)
			: this(headers)
		{
			Write(buffer, offset, count);
		}

		private int readFromChunks(byte[] buffer, int offset, int count)
		{
			int count2 = _chunks.Count;
			int num = 0;
			for (int i = 0; i < count2; i++)
			{
				Chunk chunk = _chunks[i];
				if (chunk == null)
				{
					continue;
				}
				if (chunk.ReadLeft == 0)
				{
					_chunks[i] = null;
					continue;
				}
				num += chunk.Read(buffer, offset + num, count - num);
				if (num == count)
				{
					break;
				}
			}
			return num;
		}

		private static string removeChunkExtension(string value)
		{
			int num = value.IndexOf(';');
			return (num <= -1) ? value : value.Substring(0, num);
		}

		private InputChunkState seekCrLf(byte[] buffer, ref int offset, int length)
		{
			if (!_sawCr)
			{
				if (buffer[offset++] != 13)
				{
					throwProtocolViolation("CR is expected.");
				}
				_sawCr = true;
				if (offset == length)
				{
					return InputChunkState.BodyFinished;
				}
			}
			if (buffer[offset++] != 10)
			{
				throwProtocolViolation("LF is expected.");
			}
			return InputChunkState.None;
		}

		private InputChunkState setChunkSize(byte[] buffer, ref int offset, int length)
		{
			byte b = 0;
			while (offset < length)
			{
				b = buffer[offset++];
				if (_sawCr)
				{
					if (b != 10)
					{
						throwProtocolViolation("LF is expected.");
					}
					break;
				}
				switch (b)
				{
				case 13:
					_sawCr = true;
					continue;
				case 10:
					throwProtocolViolation("LF is unexpected.");
					break;
				}
				if (b == 32)
				{
					_gotIt = true;
				}
				if (!_gotIt)
				{
					_saved.Append(b);
				}
				if (_saved.Length > 20)
				{
					throwProtocolViolation("The chunk size is too long.");
				}
			}
			if (!_sawCr || b != 10)
			{
				return InputChunkState.None;
			}
			_chunkRead = 0;
			try
			{
				_chunkSize = int.Parse(removeChunkExtension(_saved.ToString()), NumberStyles.HexNumber);
			}
			catch
			{
				throwProtocolViolation("The chunk size cannot be parsed.");
			}
			if (_chunkSize == 0)
			{
				_trailerState = 2;
				return InputChunkState.Trailer;
			}
			return InputChunkState.Body;
		}

		private InputChunkState setHeaders(byte[] buffer, ref int offset, int length)
		{
			if (_trailerState == 2 && buffer[offset] == 13 && _saved.Length == 0)
			{
				offset++;
				if (offset < length && buffer[offset] == 10)
				{
					offset++;
					return InputChunkState.None;
				}
				offset--;
			}
			while (offset < length && _trailerState < 4)
			{
				byte b = buffer[offset++];
				_saved.Append(b);
				if (_saved.Length > 4196)
				{
					throwProtocolViolation("The trailer is too long.");
				}
				if (_trailerState == 1 || _trailerState == 3)
				{
					if (b != 10)
					{
						throwProtocolViolation("LF is expected.");
					}
					_trailerState++;
					continue;
				}
				switch (b)
				{
				case 13:
					_trailerState++;
					continue;
				case 10:
					throwProtocolViolation("LF is unexpected.");
					break;
				}
				_trailerState = 0;
			}
			if (_trailerState < 4)
			{
				return InputChunkState.Trailer;
			}
			_saved.Length -= 2;
			StringReader stringReader = new StringReader(_saved.ToString());
			string text;
			while ((text = stringReader.ReadLine()) != null && text.Length > 0)
			{
				_headers.Add(text);
			}
			return InputChunkState.None;
		}

		private static void throwProtocolViolation(string message)
		{
			throw new WebException(message, null, WebExceptionStatus.ServerProtocolViolation, null);
		}

		private void write(byte[] buffer, ref int offset, int length)
		{
			if (_state == InputChunkState.None)
			{
				_state = setChunkSize(buffer, ref offset, length);
				if (_state == InputChunkState.None)
				{
					return;
				}
				_saved.Length = 0;
				_sawCr = false;
				_gotIt = false;
			}
			if (_state == InputChunkState.Body && offset < length)
			{
				_state = writeBody(buffer, ref offset, length);
				if (_state == InputChunkState.Body)
				{
					return;
				}
			}
			if (_state == InputChunkState.BodyFinished && offset < length)
			{
				_state = seekCrLf(buffer, ref offset, length);
				if (_state == InputChunkState.BodyFinished)
				{
					return;
				}
				_sawCr = false;
			}
			if (_state == InputChunkState.Trailer && offset < length)
			{
				_state = setHeaders(buffer, ref offset, length);
				if (_state == InputChunkState.Trailer)
				{
					return;
				}
				_saved.Length = 0;
			}
			if (offset < length)
			{
				write(buffer, ref offset, length);
			}
		}

		private InputChunkState writeBody(byte[] buffer, ref int offset, int length)
		{
			if (_chunkSize == 0)
			{
				return InputChunkState.BodyFinished;
			}
			int num = length - offset;
			int num2 = _chunkSize - _chunkRead;
			if (num > num2)
			{
				num = num2;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(buffer, offset, array, 0, num);
			_chunks.Add(new Chunk(array));
			offset += num;
			_chunkRead += num;
			return (_chunkRead != _chunkSize) ? InputChunkState.Body : InputChunkState.BodyFinished;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			return readFromChunks(buffer, offset, count);
		}

		public void ResetBuffer()
		{
			_chunkSize = -1;
			_chunkRead = 0;
			_chunks.Clear();
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			if (count > 0)
			{
				write(buffer, ref offset, offset + count);
			}
		}

		public void WriteAndReadBack(byte[] buffer, int offset, int count, ref int read)
		{
			Write(buffer, offset, read);
			read = readFromChunks(buffer, offset, count);
		}
	}
}
