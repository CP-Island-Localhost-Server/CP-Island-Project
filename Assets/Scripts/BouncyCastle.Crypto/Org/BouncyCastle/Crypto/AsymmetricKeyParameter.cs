namespace Org.BouncyCastle.Crypto
{
	public abstract class AsymmetricKeyParameter : ICipherParameters
	{
		private readonly bool privateKey;

		public bool IsPrivate
		{
			get
			{
				return privateKey;
			}
		}

		protected AsymmetricKeyParameter(bool privateKey)
		{
			this.privateKey = privateKey;
		}

		public override bool Equals(object obj)
		{
			AsymmetricKeyParameter asymmetricKeyParameter = obj as AsymmetricKeyParameter;
			if (asymmetricKeyParameter == null)
			{
				return false;
			}
			return Equals(asymmetricKeyParameter);
		}

		protected bool Equals(AsymmetricKeyParameter other)
		{
			return privateKey == other.privateKey;
		}

		public override int GetHashCode()
		{
			return privateKey.GetHashCode();
		}
	}
}
