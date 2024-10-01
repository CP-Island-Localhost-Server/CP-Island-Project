namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class AlertDescription
	{
		public const byte close_notify = 0;

		public const byte unexpected_message = 10;

		public const byte bad_record_mac = 20;

		public const byte decryption_failed = 21;

		public const byte record_overflow = 22;

		public const byte decompression_failure = 30;

		public const byte handshake_failure = 40;

		public const byte no_certificate = 41;

		public const byte bad_certificate = 42;

		public const byte unsupported_certificate = 43;

		public const byte certificate_revoked = 44;

		public const byte certificate_expired = 45;

		public const byte certificate_unknown = 46;

		public const byte illegal_parameter = 47;

		public const byte unknown_ca = 48;

		public const byte access_denied = 49;

		public const byte decode_error = 50;

		public const byte decrypt_error = 51;

		public const byte export_restriction = 60;

		public const byte protocol_version = 70;

		public const byte insufficient_security = 71;

		public const byte internal_error = 80;

		public const byte user_canceled = 90;

		public const byte no_renegotiation = 100;

		public const byte unsupported_extension = 110;

		public const byte certificate_unobtainable = 111;

		public const byte unrecognized_name = 112;

		public const byte bad_certificate_status_response = 113;

		public const byte bad_certificate_hash_value = 114;

		public const byte unknown_psk_identity = 115;

		public const byte inappropriate_fallback = 86;

		public static string GetName(byte alertDescription)
		{
			switch (alertDescription)
			{
			case 0:
				return "close_notify";
			case 10:
				return "unexpected_message";
			case 20:
				return "bad_record_mac";
			case 21:
				return "decryption_failed";
			case 22:
				return "record_overflow";
			case 30:
				return "decompression_failure";
			case 40:
				return "handshake_failure";
			case 41:
				return "no_certificate";
			case 42:
				return "bad_certificate";
			case 43:
				return "unsupported_certificate";
			case 44:
				return "certificate_revoked";
			case 45:
				return "certificate_expired";
			case 46:
				return "certificate_unknown";
			case 47:
				return "illegal_parameter";
			case 48:
				return "unknown_ca";
			case 49:
				return "access_denied";
			case 50:
				return "decode_error";
			case 51:
				return "decrypt_error";
			case 60:
				return "export_restriction";
			case 70:
				return "protocol_version";
			case 71:
				return "insufficient_security";
			case 80:
				return "internal_error";
			case 90:
				return "user_canceled";
			case 100:
				return "no_renegotiation";
			case 110:
				return "unsupported_extension";
			case 111:
				return "certificate_unobtainable";
			case 112:
				return "unrecognized_name";
			case 113:
				return "bad_certificate_status_response";
			case 114:
				return "bad_certificate_hash_value";
			case 115:
				return "unknown_psk_identity";
			case 86:
				return "inappropriate_fallback";
			default:
				return "UNKNOWN";
			}
		}

		public static string GetText(byte alertDescription)
		{
			return GetName(alertDescription) + "(" + alertDescription + ")";
		}
	}
}
