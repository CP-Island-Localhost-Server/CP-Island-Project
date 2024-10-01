using Sfs2X.Entities.Data;

namespace Sfs2X.Requests.MMO
{
	public class MMORoomSettings : RoomSettings
	{
		private Vec3D defaultAOI;

		private MapLimits mapLimits;

		private int userMaxLimboSeconds = 50;

		private int proximityListUpdateMillis = 250;

		private bool sendAOIEntryPoint = true;

		public Vec3D DefaultAOI
		{
			get
			{
				return defaultAOI;
			}
			set
			{
				defaultAOI = value;
			}
		}

		public MapLimits MapLimits
		{
			get
			{
				return mapLimits;
			}
			set
			{
				mapLimits = value;
			}
		}

		public int UserMaxLimboSeconds
		{
			get
			{
				return userMaxLimboSeconds;
			}
			set
			{
				userMaxLimboSeconds = value;
			}
		}

		public int ProximityListUpdateMillis
		{
			get
			{
				return proximityListUpdateMillis;
			}
			set
			{
				proximityListUpdateMillis = value;
			}
		}

		public bool SendAOIEntryPoint
		{
			get
			{
				return sendAOIEntryPoint;
			}
			set
			{
				sendAOIEntryPoint = value;
			}
		}

		public MMORoomSettings(string name)
			: base(name)
		{
		}
	}
}
