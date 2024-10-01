namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class HashAlgorithm
	{
		public const byte none = 0;

		public const byte md5 = 1;

		public const byte sha1 = 2;

		public const byte sha224 = 3;

		public const byte sha256 = 4;

		public const byte sha384 = 5;

		public const byte sha512 = 6;

		public static string GetName(byte hashAlgorithm)
		{
			switch (hashAlgorithm)
			{
			case 0:
				return "none";
			case 1:
				return "md5";
			case 2:
				return "sha1";
			case 3:
				return "sha224";
			case 4:
				return "sha256";
			case 5:
				return "sha384";
			case 6:
				return "sha512";
			default:
				return "UNKNOWN";
			}
		}

		public static string GetText(byte hashAlgorithm)
		{
			return GetName(hashAlgorithm) + "(" + hashAlgorithm + ")";
		}
	}
}
