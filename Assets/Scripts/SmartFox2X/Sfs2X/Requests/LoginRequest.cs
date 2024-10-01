using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using Sfs2X.Util;

namespace Sfs2X.Requests
{
	public class LoginRequest : BaseRequest
	{
		public static readonly string KEY_ZONE_NAME = "zn";

		public static readonly string KEY_USER_NAME = "un";

		public static readonly string KEY_PASSWORD = "pw";

		public static readonly string KEY_PARAMS = "p";

		public static readonly string KEY_PRIVILEGE_ID = "pi";

		public static readonly string KEY_ID = "id";

		public static readonly string KEY_ROOMLIST = "rl";

		public static readonly string KEY_RECONNECTION_SECONDS = "rs";

		private string zoneName;

		private string userName;

		private string password;

		private ISFSObject parameters;

		public LoginRequest(string userName, string password, string zoneName, ISFSObject parameters)
			: base(RequestType.Login)
		{
			Init(userName, password, zoneName, parameters);
		}

		public LoginRequest(string userName, string password, string zoneName)
			: base(RequestType.Login)
		{
			Init(userName, password, zoneName, null);
		}

		public LoginRequest(string userName, string password)
			: base(RequestType.Login)
		{
			Init(userName, password, null, null);
		}

		public LoginRequest(string userName)
			: base(RequestType.Login)
		{
			Init(userName, null, null, null);
		}

		private void Init(string userName, string password, string zoneName, ISFSObject parameters)
		{
			this.userName = userName;
			this.password = ((password != null) ? password : "");
			this.zoneName = zoneName;
			this.parameters = parameters;
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_ZONE_NAME, zoneName);
			sfso.PutUtfString(KEY_USER_NAME, userName);
			if (password.Length > 0)
			{
				password = PasswordUtil.MD5Password(sfs.SessionToken + password);
			}
			sfso.PutUtfString(KEY_PASSWORD, password);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_PARAMS, parameters);
			}
		}

		public override void Validate(SmartFox sfs)
		{
			if (sfs.MySelf != null)
			{
				throw new SFSValidationError("LoginRequest Error", new string[1] { "You are already logged in. Logout first" });
			}
			if ((zoneName == null || zoneName.Length == 0) && sfs.Config != null)
			{
				zoneName = sfs.Config.Zone;
			}
			if (zoneName == null || zoneName.Length == 0)
			{
				throw new SFSValidationError("LoginRequest Error", new string[1] { "Missing Zone name" });
			}
		}
	}
}
