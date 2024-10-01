using System.Collections.Specialized;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public class HttpDigestIdentity : GenericIdentity
	{
		private NameValueCollection _parameters;

		public string Algorithm
		{
			get
			{
				return _parameters["algorithm"];
			}
		}

		public string Cnonce
		{
			get
			{
				return _parameters["cnonce"];
			}
		}

		public string Nc
		{
			get
			{
				return _parameters["nc"];
			}
		}

		public string Nonce
		{
			get
			{
				return _parameters["nonce"];
			}
		}

		public string Opaque
		{
			get
			{
				return _parameters["opaque"];
			}
		}

		public string Qop
		{
			get
			{
				return _parameters["qop"];
			}
		}

		public string Realm
		{
			get
			{
				return _parameters["realm"];
			}
		}

		public string Response
		{
			get
			{
				return _parameters["response"];
			}
		}

		public string Uri
		{
			get
			{
				return _parameters["uri"];
			}
		}

		internal HttpDigestIdentity(NameValueCollection parameters)
			: base(parameters["username"], "Digest")
		{
			_parameters = parameters;
		}

		internal bool IsValid(string password, string realm, string method, string entity)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(_parameters);
			nameValueCollection["password"] = password;
			nameValueCollection["realm"] = realm;
			nameValueCollection["method"] = method;
			nameValueCollection["entity"] = entity;
			return _parameters["response"] == AuthenticationResponse.CreateRequestDigest(nameValueCollection);
		}
	}
}
