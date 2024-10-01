using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class BanUserRequest : BaseRequest
	{
		public static readonly string KEY_USER_ID = "u";

		public static readonly string KEY_MESSAGE = "m";

		public static readonly string KEY_DELAY = "d";

		public static readonly string KEY_BAN_MODE = "b";

		public static readonly string KEY_BAN_DURATION_HOURS = "dh";

		private int userId;

		private string message;

		private int delay;

		private BanMode banMode;

		private int durationHours;

		public BanUserRequest(int userId, string message, BanMode banMode, int delaySeconds, int durationHours)
			: base(RequestType.BanUser)
		{
			Init(userId, message, banMode, delaySeconds, durationHours);
		}

		public BanUserRequest(int userId)
			: base(RequestType.BanUser)
		{
			Init(userId, null, BanMode.BY_NAME, 5, 0);
		}

		public BanUserRequest(int userId, string message)
			: base(RequestType.BanUser)
		{
			Init(userId, message, BanMode.BY_NAME, 5, 0);
		}

		public BanUserRequest(int userId, string message, BanMode banMode)
			: base(RequestType.BanUser)
		{
			Init(userId, message, banMode, 5, 0);
		}

		public BanUserRequest(int userId, string message, BanMode banMode, int delaySeconds)
			: base(RequestType.BanUser)
		{
			Init(userId, message, banMode, delaySeconds, 0);
		}

		private void Init(int userId, string message, BanMode banMode, int delaySeconds, int durationHours)
		{
			this.userId = userId;
			this.message = message;
			this.banMode = banMode;
			delay = delaySeconds;
			this.durationHours = durationHours;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (list.Count > 0)
			{
				throw new SFSValidationError("BanUser request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_USER_ID, userId);
			sfso.PutInt(KEY_DELAY, delay);
			sfso.PutInt(KEY_BAN_MODE, (int)banMode);
			sfso.PutInt(KEY_BAN_DURATION_HOURS, durationHours);
			if (message != null && message.Length > 0)
			{
				sfso.PutUtfString(KEY_MESSAGE, message);
			}
		}
	}
}
