namespace ClubPenguin.Net
{
	public enum NetworkErrorType
	{
		NOT_MODIFIED = 14,
		NO_RESPONSE_FOUND = 99,
		ERROR = 700,
		DATA_NOT_FOUND = 913,
		META_LOCKED = 706,
		META_NOT_FOUND = 703,
		TOO_MANY_TRIES = 704,
		NOT_ALLOWED_IN_PRODUCTION = 705,
		MUST_NOT_SPECIFY_COMMAND_TIME = 910,
		META_INVALID = 912,
		META_WRONG_TYPE = 908,
		PREREQS_NOT_SATISFIED = 915,
		PATCH_NOT_FOUND = 708,
		CONTENT_VERIFICATION_WARNING = 711,
		MALFORMED_MANIFEST = 712,
		POORLY_SCRIPTED_CONTENT = 713,
		PLAYER_NOT_FOUND = 714,
		INVALID_GAME_CONTENT = 715,
		AB_TEST_CMS_NOT_CONFIGURED_FOR_APP = 717,
		AB_TEST_ERROR = 718,
		AB_TEST_NO_DATA = 727,
		LOGIN_TIME_MISMATCH = 917,
		LAST_LOGIN_TIME_ON_SERVER_NOT_FOUND = 949,
		PLAYER_ID_GENERATION_FAILED = 804,
		PLAYER_SECRET_ALREADY_REGISTERED = 805,
		EMPTY_CONTENT_VERSION = 806,
		NO_APPLICABLE_MANIFEST_VERSION = 807,
		CHARACTER_ENCODING_INVALID = 935,
		REQUEST_METHOD_NOT_SUPPORTED = 936,
		REQUEST_JSON_DATA_MALFORMED = 937,
		SECURITY_HASH_UNVERIFIED = 938,
		ITEM_TRANSACTION_FAILED = 939,
		ITEM_CATEGORY_UNRECOGNIZED = 940,
		SENDER_SYSTEM_UNRECOGNIZED = 942,
		TRANSACTION_ALREADY_CONSUMED = 943,
		CONTENT_TYPE_INVALID = 945,
		API_ENDPOINT_DISABLED = 946,
		COMMAND_TIMESTAMP_MISSING = 901,
		COMMAND_TIME_TOO_OLD = 903,
		COMMAND_NOT_FOUND = 904,
		COMMAND_NO_RETRY = 905,
		COMMAND_TIMESTAMP_IN_PAST = 909,
		COMMAND_TIMESTAMP_IN_FUTURE = 911,
		COMMAND_NOT_SHARED = 947,
		NOT_ENOUGH_RESOURCES = 707,
		LOCATION_ALREADY_EXISTS = 716,
		LOCATION_DOES_NOT_EXIST = 719,
		DATA_MODEL_SERIALIZATION_FAIL = 907,
		GX_UPDATE_FAILED = 954,
		PURCHASE_CANCELED = 919,
		PURCHASE_REFUNDED = 920,
		UNSUPPORTED_VENDOR_FORMAT = 921,
		PURCHASE_TOO_OLD = 922,
		PURCHASE_ALREADY_CONSUMED = 923,
		PURCHASE_TYPE_UNKNOWN = 924,
		RECEIPT_PARSE_ERROR = 926,
		BAD_APP_STORE_RESPONSE = 927,
		BAD_RECEIPT_INPUT = 928,
		INVALID_APP_STORE_USER_ID = 929,
		UNKNOWN_APP_STORE_ERROR = 930,
		INVALID_APP_SECRET = 931,
		MISSING_APP_STORE_USER_ID = 933,
		INVALID_RECEIPT_SIGNATURE = 934,
		INVALID_APP_ID_FOR_PRODUCT = 948,
		ERROR_WHILE_VERIFYING_RECEIPT = 951,
		INVALID_SUBSCRIPTION = 952,
		TRANSACTION_ID_MISMATCH = 955,
		PRODUCT_ID_MISMATCH = 956,
		NEIGHBOR_INVITE_SENT_NOT_FOUND = 760,
		NEIGHBOR_ALREADY_EXISTS = 762,
		GIFT_ACCEPT_LIMIT_REACHED = 763,
		PENDING_GIFT_NOT_FOUND = 766,
		NETWORK_NOT_SUPPORTED = 767,
		INVALID_JWT_SIGNATURE = 770,
		JWT_ISSUED_IN_FUTURE = 771,
		INVITE_CODE_MAPPING_NOT_FOUND = 774,
		CANNOT_NEIGHBOR_INVITE_SELF = 776,
		CANNOT_SEND_GIFT_TO_SELF = 777,
		TOO_MANY_NEIGHBORS = 779,
		GIFT_REQUEST_NOT_FOUND = 780,
		CANNOT_NEIGHBOR_INVITE_EXISTING = 781,
		INVALID_OAUTH_TOKEN = 782,
		SECURITY_MISSING_AUTHENTICATION = 2,
		SECURITY_CANNOT_AUTHENTICATE = 3,
		SECURITY_NOT_AUTHORIZED = 4,
		SECURITY_ACCESS_FORBIDDEN = 5,
		AUTHENTICATION_FAILED = 800,
		AUTHORIZATION_FAILED = 801,
		GOOGLE_OAUTH_FAILED = 803,
		GOOGLEPLAY_NETWORK_PROBLEM = 808,
		GOOGLE_OAUTH_SECURITY_EXCEPTION = 809,
		CLOUD_NEIGHBOR_LOOKUP_ERROR = 600,
		CLOUD_NEIGHBOR_ADD_ERROR = 601,
		CLOUD_GIFT_LOOKUP_ERROR = 602,
		CLOUD_GIFT_ADD_ERROR = 603,
		CLOUD_GIFT_DELETE_ERROR = 604,
		BAD_CLOUD_REQUEST_400 = 606,
		CLOUD_CONFLICT_409 = 607,
		CLOUD_ENDPOINT_NOT_FOUND_404 = 608,
		CLOUD_NOT_AVAILABLE_503 = 610,
		INTERNAL_CLOUD_ERROR_500 = 611,
		UNAUTHORIZED_CLOUD_REQUEST_401 = 612,
		CLOUD_REQUEST_ERROR = 614,
		UNKNOWN_CLOUD_ERROR = 616,
		L2_CACHE_TIMEOUT = 916,
		L2_CACHE_FAILURE = 953,
		MEMCACHE_NOT_AVAILABLE = 944,
		INPUT_BAD_REQUEST = 1,
		REQUIRED_PARAMETER_MISSING = 941,
		INPUT_METHOD_NOT_ALLOWED = 7,
		INPUT_MALFORMED_URI = 8,
		INPUT_INVALID_REQUEST_PARAM = 10,
		INPUT_INVALID_SERVICE_FEATURE = 11,
		GENERAL_RESOURCE_NOT_FOUND = 6,
		GENERAL_RESOURCE_NO_LONGER_AVAILABLE = 13,
		SYSTEM_INTERNAL_ERROR = 999,
		CP_ROOM_FULL = 1000,
		CP_NO_SERVER_FOUND = 1001,
		CP_IGLOO_WRONG_ROOM = 1009,
		CP_IGLOO_UNAVAILABLE = 1010,
		CP_CAPTCHA_REQUEST_FLOODING = 1021,
		CP_INVALID_CAPTCHA_DIMENSIONS = 1022,
		CP_INVALID_CAPTCHA_SOLUTION = 0x3FF,
		GENERAL_ERROR = 10000,
		REQUEST_TIME_OUT = 10001,
		JSON_ERROR_PARSING_FAILURE = 10002
	}
}
