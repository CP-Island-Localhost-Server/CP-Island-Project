using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;
using Sfs2X.Requests.MMO;

namespace Sfs2X.Requests
{
	public class CreateRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";

		public static readonly string KEY_NAME = "n";

		public static readonly string KEY_PASSWORD = "p";

		public static readonly string KEY_GROUP_ID = "g";

		public static readonly string KEY_ISGAME = "ig";

		public static readonly string KEY_MAXUSERS = "mu";

		public static readonly string KEY_MAXSPECTATORS = "ms";

		public static readonly string KEY_MAXVARS = "mv";

		public static readonly string KEY_ROOMVARS = "rv";

		public static readonly string KEY_PERMISSIONS = "pm";

		public static readonly string KEY_EVENTS = "ev";

		public static readonly string KEY_EXTID = "xn";

		public static readonly string KEY_EXTCLASS = "xc";

		public static readonly string KEY_EXTPROP = "xp";

		public static readonly string KEY_AUTOJOIN = "aj";

		public static readonly string KEY_ROOM_TO_LEAVE = "rl";

		public static readonly string KEY_ALLOW_JOIN_INVITATION_BY_OWNER = "aji";

		public static readonly string KEY_MMO_DEFAULT_AOI = "maoi";

		public static readonly string KEY_MMO_MAP_LOW_LIMIT = "mllm";

		public static readonly string KEY_MMO_MAP_HIGH_LIMIT = "mlhm";

		public static readonly string KEY_MMO_USER_MAX_LIMBO_SECONDS = "muls";

		public static readonly string KEY_MMO_PROXIMITY_UPDATE_MILLIS = "mpum";

		public static readonly string KEY_MMO_SEND_ENTRY_POINT = "msep";

		private RoomSettings settings;

		private bool autoJoin;

		private Room roomToLeave;

		public CreateRoomRequest(RoomSettings settings, bool autoJoin, Room roomToLeave)
			: base(RequestType.CreateRoom)
		{
			Init(settings, autoJoin, roomToLeave);
		}

		public CreateRoomRequest(RoomSettings settings, bool autoJoin)
			: base(RequestType.CreateRoom)
		{
			Init(settings, autoJoin, null);
		}

		public CreateRoomRequest(RoomSettings settings)
			: base(RequestType.CreateRoom)
		{
			Init(settings, false, null);
		}

		private void Init(RoomSettings settings, bool autoJoin, Room roomToLeave)
		{
			this.settings = settings;
			this.autoJoin = autoJoin;
			this.roomToLeave = roomToLeave;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (settings.Name == null || settings.Name.Length == 0)
			{
				list.Add("Missing room name");
			}
			if (settings.MaxUsers <= 0)
			{
				list.Add("maxUsers must be > 0");
			}
			if (settings.Extension != null)
			{
				if (settings.Extension.ClassName == null || settings.Extension.ClassName.Length == 0)
				{
					list.Add("Missing Extension class name");
				}
				if (settings.Extension.Id == null || settings.Extension.Id.Length == 0)
				{
					list.Add("Missing Extension id");
				}
			}
			if (settings is MMORoomSettings)
			{
				MMORoomSettings mMORoomSettings = settings as MMORoomSettings;
				if (mMORoomSettings.DefaultAOI == null)
				{
					list.Add("Missing default AOI (area of interest)");
				}
				if (mMORoomSettings.MapLimits != null && (mMORoomSettings.MapLimits.LowerLimit == null || mMORoomSettings.MapLimits.HigherLimit == null))
				{
					list.Add("Map limits must be both defined");
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateRoom request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_NAME, settings.Name);
			sfso.PutUtfString(KEY_GROUP_ID, settings.GroupId);
			sfso.PutUtfString(KEY_PASSWORD, settings.Password);
			sfso.PutBool(KEY_ISGAME, settings.IsGame);
			sfso.PutShort(KEY_MAXUSERS, settings.MaxUsers);
			sfso.PutShort(KEY_MAXSPECTATORS, settings.MaxSpectators);
			sfso.PutShort(KEY_MAXVARS, settings.MaxVariables);
			sfso.PutBool(KEY_ALLOW_JOIN_INVITATION_BY_OWNER, settings.AllowOwnerOnlyInvitation);
			if (settings.Variables != null && settings.Variables.Count > 0)
			{
				ISFSArray iSFSArray = SFSArray.NewInstance();
				foreach (RoomVariable variable in settings.Variables)
				{
					if (variable is RoomVariable)
					{
						RoomVariable roomVariable = variable as RoomVariable;
						iSFSArray.AddSFSArray(roomVariable.ToSFSArray());
					}
				}
				sfso.PutSFSArray(KEY_ROOMVARS, iSFSArray);
			}
			if (settings.Permissions != null)
			{
				List<bool> list = new List<bool>();
				list.Add(settings.Permissions.AllowNameChange);
				list.Add(settings.Permissions.AllowPasswordStateChange);
				list.Add(settings.Permissions.AllowPublicMessages);
				list.Add(settings.Permissions.AllowResizing);
				sfso.PutBoolArray(KEY_PERMISSIONS, list.ToArray());
			}
			if (settings.Events != null)
			{
				List<bool> list2 = new List<bool>();
				list2.Add(settings.Events.AllowUserEnter);
				list2.Add(settings.Events.AllowUserExit);
				list2.Add(settings.Events.AllowUserCountChange);
				list2.Add(settings.Events.AllowUserVariablesUpdate);
				sfso.PutBoolArray(KEY_EVENTS, list2.ToArray());
			}
			if (settings.Extension != null)
			{
				sfso.PutUtfString(KEY_EXTID, settings.Extension.Id);
				sfso.PutUtfString(KEY_EXTCLASS, settings.Extension.ClassName);
				if (settings.Extension.PropertiesFile != null && settings.Extension.PropertiesFile.Length > 0)
				{
					sfso.PutUtfString(KEY_EXTPROP, settings.Extension.PropertiesFile);
				}
			}
			if (settings is MMORoomSettings)
			{
				MMORoomSettings mMORoomSettings = settings as MMORoomSettings;
				if (mMORoomSettings.DefaultAOI.IsFloat())
				{
					sfso.PutFloatArray(KEY_MMO_DEFAULT_AOI, mMORoomSettings.DefaultAOI.ToFloatArray());
					if (mMORoomSettings.MapLimits != null)
					{
						sfso.PutFloatArray(KEY_MMO_MAP_LOW_LIMIT, mMORoomSettings.MapLimits.LowerLimit.ToFloatArray());
						sfso.PutFloatArray(KEY_MMO_MAP_HIGH_LIMIT, mMORoomSettings.MapLimits.HigherLimit.ToFloatArray());
					}
				}
				else
				{
					sfso.PutIntArray(KEY_MMO_DEFAULT_AOI, mMORoomSettings.DefaultAOI.ToIntArray());
					if (mMORoomSettings.MapLimits != null)
					{
						sfso.PutIntArray(KEY_MMO_MAP_LOW_LIMIT, mMORoomSettings.MapLimits.LowerLimit.ToIntArray());
						sfso.PutIntArray(KEY_MMO_MAP_HIGH_LIMIT, mMORoomSettings.MapLimits.HigherLimit.ToIntArray());
					}
				}
				sfso.PutShort(KEY_MMO_USER_MAX_LIMBO_SECONDS, (short)mMORoomSettings.UserMaxLimboSeconds);
				sfso.PutShort(KEY_MMO_PROXIMITY_UPDATE_MILLIS, (short)mMORoomSettings.ProximityListUpdateMillis);
				sfso.PutBool(KEY_MMO_SEND_ENTRY_POINT, mMORoomSettings.SendAOIEntryPoint);
			}
			sfso.PutBool(KEY_AUTOJOIN, autoJoin);
			if (roomToLeave != null)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomToLeave.Id);
			}
		}
	}
}
