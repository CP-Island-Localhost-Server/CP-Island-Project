namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemHeader
	{
		private string name;

		private string val;

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual string Value
		{
			get
			{
				return val;
			}
		}

		public PemHeader(string name, string val)
		{
			this.name = name;
			this.val = val;
		}

		public override int GetHashCode()
		{
			return GetHashCode(name) + 31 * GetHashCode(val);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is PemHeader))
			{
				return false;
			}
			PemHeader pemHeader = (PemHeader)obj;
			if (object.Equals(name, pemHeader.name))
			{
				return object.Equals(val, pemHeader.val);
			}
			return false;
		}

		private int GetHashCode(string s)
		{
			if (s == null)
			{
				return 1;
			}
			return s.GetHashCode();
		}
	}
}
