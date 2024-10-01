using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public class HttpBasicIdentity : GenericIdentity
	{
		private string _password;

		public virtual string Password
		{
			get
			{
				return _password;
			}
		}

		internal HttpBasicIdentity(string username, string password)
			: base(username, "Basic")
		{
			_password = password;
		}
	}
}
