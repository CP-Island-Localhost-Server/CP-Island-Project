using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public sealed class PgpPad
	{
		private PgpPad()
		{
		}

		public static byte[] PadSessionData(byte[] sessionInfo)
		{
			byte[] array = new byte[40];
			Array.Copy(sessionInfo, 0, array, 0, sessionInfo.Length);
			byte b = (byte)(array.Length - sessionInfo.Length);
			for (int i = sessionInfo.Length; i != array.Length; i++)
			{
				array[i] = b;
			}
			return array;
		}

		public static byte[] UnpadSessionData(byte[] encoded)
		{
			byte b = encoded[encoded.Length - 1];
			for (int i = encoded.Length - b; i != encoded.Length; i++)
			{
				if (encoded[i] != b)
				{
					throw new PgpException("bad padding found in session data");
				}
			}
			byte[] array = new byte[encoded.Length - b];
			Array.Copy(encoded, 0, array, 0, array.Length);
			return array;
		}
	}
}
