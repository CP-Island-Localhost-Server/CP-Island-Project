namespace Sfs2X.Requests
{
	public class RoomPermissions
	{
		private bool allowNameChange;

		private bool allowPasswordStateChange;

		private bool allowPublicMessages;

		private bool allowResizing;

		public bool AllowNameChange
		{
			get
			{
				return allowNameChange;
			}
			set
			{
				allowNameChange = value;
			}
		}

		public bool AllowPasswordStateChange
		{
			get
			{
				return allowPasswordStateChange;
			}
			set
			{
				allowPasswordStateChange = value;
			}
		}

		public bool AllowPublicMessages
		{
			get
			{
				return allowPublicMessages;
			}
			set
			{
				allowPublicMessages = value;
			}
		}

		public bool AllowResizing
		{
			get
			{
				return allowResizing;
			}
			set
			{
				allowResizing = value;
			}
		}
	}
}
