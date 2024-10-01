using System;
using System.IO;
using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpLiteralData : PgpObject
	{
		public const char Binary = 'b';

		public const char Text = 't';

		public const char Utf8 = 'u';

		public const string Console = "_CONSOLE";

		private LiteralDataPacket data;

		public int Format
		{
			get
			{
				return data.Format;
			}
		}

		public string FileName
		{
			get
			{
				return data.FileName;
			}
		}

		public DateTime ModificationTime
		{
			get
			{
				return DateTimeUtilities.UnixMsToDateTime(data.ModificationTime);
			}
		}

		public PgpLiteralData(BcpgInputStream bcpgInput)
		{
			data = (LiteralDataPacket)bcpgInput.ReadPacket();
		}

		public byte[] GetRawFileName()
		{
			return data.GetRawFileName();
		}

		public Stream GetInputStream()
		{
			return data.GetInputStream();
		}

		public Stream GetDataStream()
		{
			return GetInputStream();
		}
	}
}
