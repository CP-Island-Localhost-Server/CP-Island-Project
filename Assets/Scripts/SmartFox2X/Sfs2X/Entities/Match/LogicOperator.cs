namespace Sfs2X.Entities.Match
{
	public class LogicOperator
	{
		public static readonly LogicOperator AND = new LogicOperator("AND");

		public static readonly LogicOperator OR = new LogicOperator("OR");

		private string id;

		public string Id
		{
			get
			{
				return id;
			}
		}

		public LogicOperator(string id)
		{
			this.id = id;
		}
	}
}
