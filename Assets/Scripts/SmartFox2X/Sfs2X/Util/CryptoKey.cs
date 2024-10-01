namespace Sfs2X.Util
{
	public class CryptoKey
	{
		private ByteArray iv;

		private ByteArray key;

		public ByteArray IV
		{
			get
			{
				return iv;
			}
		}

		public ByteArray Key
		{
			get
			{
				return key;
			}
		}

		public CryptoKey(ByteArray iv, ByteArray key)
		{
			this.iv = iv;
			this.key = key;
		}
	}
}
