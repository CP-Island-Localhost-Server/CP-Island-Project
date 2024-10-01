namespace Sfs2X.Requests
{
	public class HandshakeRequest : BaseRequest, IRequest
	{
		public static readonly string KEY_SESSION_TOKEN = "tk";

		public static readonly string KEY_API = "api";

		public static readonly string KEY_COMPRESSION_THRESHOLD = "ct";

		public static readonly string KEY_RECONNECTION_TOKEN = "rt";

		public static readonly string KEY_CLIENT_TYPE = "cl";

		public static readonly string KEY_MAX_MESSAGE_SIZE = "ms";

		public HandshakeRequest(string apiVersion, string reconnectionToken, string clientDetails)
			: base(RequestType.Handshake)
		{
			sfso.PutUtfString(KEY_API, apiVersion);
			sfso.PutUtfString(KEY_CLIENT_TYPE, clientDetails);
			if (reconnectionToken != null)
			{
				sfso.PutUtfString(KEY_RECONNECTION_TOKEN, reconnectionToken);
			}
		}
	}
}
