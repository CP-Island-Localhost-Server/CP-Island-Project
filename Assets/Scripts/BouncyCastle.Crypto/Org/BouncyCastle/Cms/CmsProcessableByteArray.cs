using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsProcessableByteArray : CmsProcessable, CmsReadable
	{
		private readonly byte[] bytes;

		public CmsProcessableByteArray(byte[] bytes)
		{
			this.bytes = bytes;
		}

		public virtual Stream GetInputStream()
		{
			return new MemoryStream(bytes, false);
		}

		public virtual void Write(Stream zOut)
		{
			zOut.Write(bytes, 0, bytes.Length);
		}

		[Obsolete]
		public virtual object GetContent()
		{
			return bytes.Clone();
		}
	}
}
