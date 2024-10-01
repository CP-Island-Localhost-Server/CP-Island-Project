using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib
{
	public class ZOutputStream : Stream
	{
		private const int BufferSize = 512;

		protected ZStream z;

		protected int flushLevel = 0;

		protected byte[] buf = new byte[512];

		protected byte[] buf1 = new byte[1];

		protected bool compress;

		protected Stream output;

		protected bool closed;

		public sealed override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public sealed override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public sealed override bool CanWrite
		{
			get
			{
				return !closed;
			}
		}

		public virtual int FlushMode
		{
			get
			{
				return flushLevel;
			}
			set
			{
				flushLevel = value;
			}
		}

		public sealed override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public sealed override long Position
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

		public virtual long TotalIn
		{
			get
			{
				return z.total_in;
			}
		}

		public virtual long TotalOut
		{
			get
			{
				return z.total_out;
			}
		}

		public ZOutputStream(Stream output)
			: this(output, null)
		{
		}

		public ZOutputStream(Stream output, ZStream z)
		{
			if (z == null)
			{
				z = new ZStream();
				z.inflateInit();
			}
			this.output = output;
			this.z = z;
			compress = false;
		}

		public ZOutputStream(Stream output, int level)
			: this(output, level, false)
		{
		}

		public ZOutputStream(Stream output, int level, bool nowrap)
		{
			this.output = output;
			z = new ZStream();
			z.deflateInit(level, nowrap);
			compress = true;
		}

		public override void Close()
		{
			if (!closed)
			{
				DoClose();
				base.Close();
			}
		}

		private void DoClose()
		{
			try
			{
				Finish();
			}
			catch (IOException)
			{
			}
			finally
			{
				closed = true;
				End();
				Platform.Dispose(output);
				output = null;
			}
		}

		public virtual void End()
		{
			if (z != null)
			{
				if (compress)
				{
					z.deflateEnd();
				}
				else
				{
					z.inflateEnd();
				}
				z.free();
				z = null;
			}
		}

		public virtual void Finish()
		{
			do
			{
				z.next_out = buf;
				z.next_out_index = 0;
				z.avail_out = buf.Length;
				int num = (compress ? z.deflate(4) : z.inflate(4));
				if (num != 1 && num != 0)
				{
					throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);
				}
				int num2 = buf.Length - z.avail_out;
				if (num2 > 0)
				{
					output.Write(buf, 0, num2);
				}
			}
			while (z.avail_in > 0 || z.avail_out == 0);
			Flush();
		}

		public override void Flush()
		{
			output.Flush();
		}

		public sealed override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public sealed override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public sealed override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] b, int off, int len)
		{
			if (len == 0)
			{
				return;
			}
			z.next_in = b;
			z.next_in_index = off;
			z.avail_in = len;
			do
			{
				z.next_out = buf;
				z.next_out_index = 0;
				z.avail_out = buf.Length;
				if ((compress ? z.deflate(flushLevel) : z.inflate(flushLevel)) != 0)
				{
					throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);
				}
				output.Write(buf, 0, buf.Length - z.avail_out);
			}
			while (z.avail_in > 0 || z.avail_out == 0);
		}

		public override void WriteByte(byte b)
		{
			buf1[0] = b;
			Write(buf1, 0, 1);
		}
	}
}
