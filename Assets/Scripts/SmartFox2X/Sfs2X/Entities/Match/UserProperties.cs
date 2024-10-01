using System;

namespace Sfs2X.Entities.Match
{
	public class UserProperties
	{
		public static readonly string NAME = "${N}";

		public static readonly string IS_PLAYER = "${ISP}";

		public static readonly string IS_SPECTATOR = "${ISS}";

		public static readonly string IS_NPC = "${ISN}";

		public static readonly string PRIVILEGE_ID = "${PRID}";

		public static readonly string IS_IN_ANY_ROOM = "${IAR}";

		public UserProperties()
		{
			throw new ArgumentException("This class cannot be instantiated");
		}
	}
}
