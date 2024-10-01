using System;
using System.IO;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class ResponseStream : Stream
	{
		private static byte[] _crlf = new byte[2] { 13, 10 };

		private bool _disposed;

		private bool _ignoreErrors;

		private HttpListenerResponse _response;

		private Stream _stream;

		private bool _trailerSent;

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal ResponseStream(Stream stream, HttpListenerResponse response, bool ignoreErrors)
		{
			_stream = stream;
			_response = response;
			_ignoreErrors = ignoreErrors;
		}

		private static byte[] getChunkSizeBytes(int size, bool final)
		{
			return Encoding.ASCII.GetBytes(string.Format("{0:x}\r\n{1}", size, (!final) ? "" : "\r\n"));
		}

		private MemoryStream getHeaders(bool closing)
		{
			if (_response.HeadersSent)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			_response.SendHeaders(memoryStream, closing);
			return memoryStream;
		}

		internal void WriteInternally(byte[] buffer, int offset, int count)
		{
			if (_ignoreErrors)
			{
				try
				{
					_stream.Write(buffer, offset, count);
					return;
				}
				catch
				{
					return;
				}
			}
			_stream.Write(buffer, offset, count);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			MemoryStream headers = getHeaders(false);
			bool sendChunked = _response.SendChunked;
			byte[] array = null;
			if (headers != null)
			{
				using (headers)
				{
					long position = headers.Position;
					headers.Position = headers.Length;
					if (sendChunked)
					{
						array = getChunkSizeBytes(count, false);
						headers.Write(array, 0, array.Length);
					}
					headers.Write(buffer, offset, count);
					buffer = headers.GetBuffer();
					offset = (int)position;
					count = (int)(headers.Position - position);
				}
			}
			else if (sendChunked)
			{
				array = getChunkSizeBytes(count, false);
				WriteInternally(array, 0, array.Length);
			}
			return _stream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void Close()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			MemoryStream headers = getHeaders(true);
			bool sendChunked = _response.SendChunked;
			byte[] array = null;
			if (headers != null)
			{
				using (headers)
				{
					long position = headers.Position;
					if (sendChunked && !_trailerSent)
					{
						array = getChunkSizeBytes(0, true);
						headers.Position = headers.Length;
						headers.Write(array, 0, array.Length);
					}
					WriteInternally(headers.GetBuffer(), (int)position, (int)(headers.Length - position));
				}
				_trailerSent = true;
			}
			else if (sendChunked && !_trailerSent)
			{
				array = getChunkSizeBytes(0, true);
				WriteInternally(array, 0, array.Length);
				_trailerSent = true;
			}
			_response.Close();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			Action<IAsyncResult> action = delegate(IAsyncResult ares)
			{
				_stream.EndWrite(ares);
				if (_response.SendChunked)
				{
					_stream.Write(_crlf, 0, 2);
				}
			};
			if (_ignoreErrors)
			{
				try
				{
					action(asyncResult);
					return;
				}
				catch
				{
					return;
				}
			}
			action(asyncResult);
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			MemoryStream headers = getHeaders(false);
			bool sendChunked = _response.SendChunked;
			byte[] array = null;
			if (headers != null)
			{
				using (headers)
				{
					long position = headers.Position;
					headers.Position = headers.Length;
					if (sendChunked)
					{
						array = getChunkSizeBytes(count, false);
						headers.Write(array, 0, array.Length);
					}
					int num = Math.Min(count, 16384 - (int)headers.Position + (int)position);
					headers.Write(buffer, offset, num);
					count -= num;
					offset += num;
					WriteInternally(headers.GetBuffer(), (int)position, (int)(headers.Length - position));
				}
			}
			else if (sendChunked)
			{
				array = getChunkSizeBytes(count, false);
				WriteInternally(array, 0, array.Length);
			}
			if (count > 0)
			{
				WriteInternally(buffer, offset, count);
			}
			if (sendChunked)
			{
				WriteInternally(_crlf, 0, 2);
			}
		}
	}
}
