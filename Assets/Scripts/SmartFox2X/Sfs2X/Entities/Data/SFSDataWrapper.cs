namespace Sfs2X.Entities.Data
{
	public class SFSDataWrapper
	{
		private int type;

		private object data;

		public int Type
		{
			get
			{
				return type;
			}
		}

		public object Data
		{
			get
			{
				return data;
			}
		}

		public SFSDataWrapper(int type, object data)
		{
			this.type = type;
			this.data = data;
		}

		public SFSDataWrapper(SFSDataType tp, object data)
		{
			type = (int)tp;
			this.data = data;
		}
	}
}
