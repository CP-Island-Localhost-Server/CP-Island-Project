using System;
using System.IO;

namespace WebSocketSharp.Net
{
	internal class ChunkedRequestStream : RequestStream
	{
		private const int _bufferLength = 8192;

		private HttpListenerContext _context;

		private ChunkStream _decoder;

		private bool _disposed;

		private bool _noMoreData;

		internal ChunkStream Decoder
		{
			get
			{
				return _decoder;
			}
			set
			{
				_decoder = value;
			}
		}

		internal ChunkedRequestStream(Stream stream, byte[] buffer, int offset, int count, HttpListenerContext context)
			: base(stream, buffer, offset, count)
		{
			_context = context;
			_decoder = new ChunkStream((WebHeaderCollection)context.Request.Headers);
		}

		private void onRead(IAsyncResult asyncResult)
		{
			ReadBufferState readBufferState = (ReadBufferState)asyncResult.AsyncState;
			HttpStreamAsyncResult asyncResult2 = readBufferState.AsyncResult;
			try
			{
				int count = base.EndRead(asyncResult);
				_decoder.Write(asyncResult2.Buffer, asyncResult2.Offset, count);
				count = _decoder.Read(readBufferState.Buffer, readBufferState.Offset, readBufferState.Count);
				readBufferState.Offset += count;
				readBufferState.Count -= count;
				if (readBufferState.Count == 0 || !_decoder.WantMore || count == 0)
				{
					_noMoreData = !_decoder.WantMore && count == 0;
					asyncResult2.Count = readBufferState.InitialCount - readBufferState.Count;
					asyncResult2.Complete();
				}
				else
				{
					asyncResult2.Offset = 0;
					asyncResult2.Count = Math.Min(8192, _decoder.ChunkLeft + 6);
					base.BeginRead(asyncResult2.Buffer, asyncResult2.Offset, asyncResult2.Count, (AsyncCallback)onRead, (object)readBufferState);
				}
			}
			catch (Exception ex)
			{
				_context.Connection.SendError(ex.Message, 400);
				asyncResult2.Complete(ex);
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "A negative value.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "A negative value.");
			}
			int num = buffer.Length;
			if (offset + count > num)
			{
				throw new ArgumentException("The sum of 'offset' and 'count' is greater than 'buffer' length.");
			}
			HttpStreamAsyncResult httpStreamAsyncResult = new HttpStreamAsyncResult(callback, state);
			if (_noMoreData)
			{
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			int num2 = _decoder.Read(buffer, offset, count);
			offset += num2;
			count -= num2;
			if (count == 0)
			{
				httpStreamAsyncResult.Count = num2;
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			if (!_decoder.WantMore)
			{
				_noMoreData = num2 == 0;
				httpStreamAsyncResult.Count = num2;
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			httpStreamAsyncResult.Buffer = new byte[8192];
			httpStreamAsyncResult.Offset = 0;
			httpStreamAsyncResult.Count = 8192;
			ReadBufferState readBufferState = new ReadBufferState(buffer, offset, count, httpStreamAsyncResult);
			readBufferState.InitialCount += num2;
			base.BeginRead(httpStreamAsyncResult.Buffer, httpStreamAsyncResult.Offset, httpStreamAsyncResult.Count, (AsyncCallback)onRead, (object)readBufferState);
			return httpStreamAsyncResult;
		}

		public override void Close()
		{
			if (!_disposed)
			{
				_disposed = true;
				base.Close();
			}
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			HttpStreamAsyncResult httpStreamAsyncResult = asyncResult as HttpStreamAsyncResult;
			if (httpStreamAsyncResult == null)
			{
				throw new ArgumentException("A wrong IAsyncResult.", "asyncResult");
			}
			if (!httpStreamAsyncResult.IsCompleted)
			{
				httpStreamAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (httpStreamAsyncResult.HasException)
			{
				throw new HttpListenerException(400, "I/O operation aborted.");
			}
			return httpStreamAsyncResult.Count;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			IAsyncResult asyncResult = BeginRead(buffer, offset, count, null, null);
			return EndRead(asyncResult);
		}
	}
}
