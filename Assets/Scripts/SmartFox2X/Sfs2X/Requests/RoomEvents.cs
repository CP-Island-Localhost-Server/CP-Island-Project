namespace Sfs2X.Requests
{
	public class RoomEvents
	{
		private bool allowUserEnter;

		private bool allowUserExit;

		private bool allowUserCountChange;

		private bool allowUserVariablesUpdate;

		public bool AllowUserEnter
		{
			get
			{
				return allowUserEnter;
			}
			set
			{
				allowUserEnter = value;
			}
		}

		public bool AllowUserExit
		{
			get
			{
				return allowUserExit;
			}
			set
			{
				allowUserExit = value;
			}
		}

		public bool AllowUserCountChange
		{
			get
			{
				return allowUserCountChange;
			}
			set
			{
				allowUserCountChange = value;
			}
		}

		public bool AllowUserVariablesUpdate
		{
			get
			{
				return allowUserVariablesUpdate;
			}
			set
			{
				allowUserVariablesUpdate = value;
			}
		}

		public RoomEvents()
		{
			allowUserEnter = false;
			allowUserExit = false;
			allowUserCountChange = false;
			allowUserVariablesUpdate = false;
		}
	}
}
