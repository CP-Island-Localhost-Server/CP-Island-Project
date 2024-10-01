namespace Org.BouncyCastle.Asn1.BC
{
	public abstract class BCObjectIdentifiers
	{
		public static readonly DerObjectIdentifier bc = new DerObjectIdentifier("1.3.6.1.4.1.22554");

		public static readonly DerObjectIdentifier bc_pbe = new DerObjectIdentifier(string.Concat(bc, ".1"));

		public static readonly DerObjectIdentifier bc_pbe_sha1 = new DerObjectIdentifier(string.Concat(bc_pbe, ".1"));

		public static readonly DerObjectIdentifier bc_pbe_sha256 = new DerObjectIdentifier(string.Concat(bc_pbe, ".2.1"));

		public static readonly DerObjectIdentifier bc_pbe_sha384 = new DerObjectIdentifier(string.Concat(bc_pbe, ".2.2"));

		public static readonly DerObjectIdentifier bc_pbe_sha512 = new DerObjectIdentifier(string.Concat(bc_pbe, ".2.3"));

		public static readonly DerObjectIdentifier bc_pbe_sha224 = new DerObjectIdentifier(string.Concat(bc_pbe, ".2.4"));

		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs5 = new DerObjectIdentifier(string.Concat(bc_pbe_sha1, ".1"));

		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12 = new DerObjectIdentifier(string.Concat(bc_pbe_sha1, ".2"));

		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs5 = new DerObjectIdentifier(string.Concat(bc_pbe_sha256, ".1"));

		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12 = new DerObjectIdentifier(string.Concat(bc_pbe_sha256, ".2"));

		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes128_cbc = new DerObjectIdentifier(string.Concat(bc_pbe_sha1_pkcs12, ".1.2"));

		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes192_cbc = new DerObjectIdentifier(string.Concat(bc_pbe_sha1_pkcs12, ".1.22"));

		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes256_cbc = new DerObjectIdentifier(string.Concat(bc_pbe_sha1_pkcs12, ".1.42"));

		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes128_cbc = new DerObjectIdentifier(string.Concat(bc_pbe_sha256_pkcs12, ".1.2"));

		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes192_cbc = new DerObjectIdentifier(string.Concat(bc_pbe_sha256_pkcs12, ".1.22"));

		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes256_cbc = new DerObjectIdentifier(string.Concat(bc_pbe_sha256_pkcs12, ".1.42"));
	}
}
