using System.IO;
using System.Security.Cryptography;
using Sfs2X.Bitswarm;
using Sfs2X.Util;

namespace Sfs2X.Core
{
	public class DefaultPacketEncrypter : IPacketEncrypter
	{
		private BitSwarmClient bitSwarm;

		public DefaultPacketEncrypter(BitSwarmClient bitSwarm)
		{
			this.bitSwarm = bitSwarm;
		}

		public void Encrypt(ByteArray data)
		{
			CryptoKey cryptoKey = bitSwarm.CryptoKey;
			byte[] bytes;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Key = cryptoKey.Key.Bytes;
				rijndaelManaged.IV = cryptoKey.IV.Bytes;
				ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(data.Bytes, 0, data.Length);
						cryptoStream.FlushFinalBlock();
					}
					bytes = memoryStream.ToArray();
				}
			}
			data.Bytes = bytes;
		}

		public void Decrypt(ByteArray data)
		{
			CryptoKey cryptoKey = bitSwarm.CryptoKey;
			byte[] bytes;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Key = cryptoKey.Key.Bytes;
				rijndaelManaged.IV = cryptoKey.IV.Bytes;
				ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(data.Bytes, 0, data.Bytes.Length);
						cryptoStream.FlushFinalBlock();
					}
					bytes = memoryStream.ToArray();
				}
			}
			data.Bytes = bytes;
		}
	}
}
