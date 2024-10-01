using System.IO;
using Org.BouncyCastle.Apache.Bzip2;
using Org.BouncyCastle.Utilities.Zlib;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpCompressedData : PgpObject
	{
		private readonly CompressedDataPacket data;

		public CompressionAlgorithmTag Algorithm
		{
			get
			{
				return data.Algorithm;
			}
		}

		public PgpCompressedData(BcpgInputStream bcpgInput)
		{
			data = (CompressedDataPacket)bcpgInput.ReadPacket();
		}

		public Stream GetInputStream()
		{
			return data.GetInputStream();
		}

		public Stream GetDataStream()
		{
			switch (Algorithm)
			{
			case CompressionAlgorithmTag.Uncompressed:
				return GetInputStream();
			case CompressionAlgorithmTag.Zip:
				return new ZInputStream(GetInputStream(), true);
			case CompressionAlgorithmTag.ZLib:
				return new ZInputStream(GetInputStream());
			case CompressionAlgorithmTag.BZip2:
				return new CBZip2InputStream(GetInputStream());
			default:
				throw new PgpException("can't recognise compression algorithm: " + Algorithm);
			}
		}
	}
}
