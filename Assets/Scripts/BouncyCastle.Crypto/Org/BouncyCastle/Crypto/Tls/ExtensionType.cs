namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class ExtensionType
	{
		public const int server_name = 0;

		public const int max_fragment_length = 1;

		public const int client_certificate_url = 2;

		public const int trusted_ca_keys = 3;

		public const int truncated_hmac = 4;

		public const int status_request = 5;

		public const int user_mapping = 6;

		public const int elliptic_curves = 10;

		public const int ec_point_formats = 11;

		public const int srp = 12;

		public const int signature_algorithms = 13;

		public const int use_srtp = 14;

		public const int heartbeat = 15;

		public const int encrypt_then_mac = 22;

		public const int extended_master_secret = 23;

		public const int session_ticket = 35;

		public const int renegotiation_info = 65281;

		public static readonly int negotiated_ff_dhe_groups = 101;
	}
}
