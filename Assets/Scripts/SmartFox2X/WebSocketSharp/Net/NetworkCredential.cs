using System;

namespace WebSocketSharp.Net
{
	public class NetworkCredential
	{
		private string _domain;

		private string _password;

		private string[] _roles;

		private string _username;

		public string Domain
		{
			get
			{
				return _domain ?? string.Empty;
			}
			internal set
			{
				_domain = value;
			}
		}

		public string Password
		{
			get
			{
				return _password ?? string.Empty;
			}
			internal set
			{
				_password = value;
			}
		}

		public string[] Roles
		{
			get
			{
				return _roles;
			}
			internal set
			{
				_roles = value;
			}
		}

		public string UserName
		{
			get
			{
				return _username;
			}
			internal set
			{
				_username = value;
			}
		}

		public NetworkCredential(string username, string password)
			: this(username, password, null)
		{
		}

		public NetworkCredential(string username, string password, string domain, params string[] roles)
		{
			if (username == null || username.Length == 0)
			{
				throw new ArgumentException("Must not be null or empty.", "username");
			}
			_username = username;
			_password = password;
			_domain = domain;
			_roles = roles;
		}
	}
}
