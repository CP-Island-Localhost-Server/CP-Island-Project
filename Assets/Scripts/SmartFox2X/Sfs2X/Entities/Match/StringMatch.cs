namespace Sfs2X.Entities.Match
{
	public class StringMatch : IMatcher
	{
		private static readonly int TYPE_ID = 2;

		public static readonly StringMatch EQUALS = new StringMatch("==");

		public static readonly StringMatch NOT_EQUALS = new StringMatch("!=");

		public static readonly StringMatch CONTAINS = new StringMatch("contains");

		public static readonly StringMatch STARTS_WITH = new StringMatch("startsWith");

		public static readonly StringMatch ENDS_WITH = new StringMatch("endsWith");

		private string symbol;

		public string Symbol
		{
			get
			{
				return symbol;
			}
		}

		public int Type
		{
			get
			{
				return TYPE_ID;
			}
		}

		public StringMatch(string symbol)
		{
			this.symbol = symbol;
		}
	}
}
