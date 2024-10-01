using System;
using System.Collections;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Pkcs
{
	public class AsymmetricKeyEntry : Pkcs12Entry
	{
		private readonly AsymmetricKeyParameter key;

		public AsymmetricKeyParameter Key
		{
			get
			{
				return key;
			}
		}

		public AsymmetricKeyEntry(AsymmetricKeyParameter key)
			: base(Platform.CreateHashtable())
		{
			this.key = key;
		}

		[Obsolete]
		public AsymmetricKeyEntry(AsymmetricKeyParameter key, Hashtable attributes)
			: base(attributes)
		{
			this.key = key;
		}

		public AsymmetricKeyEntry(AsymmetricKeyParameter key, IDictionary attributes)
			: base(attributes)
		{
			this.key = key;
		}

		public override bool Equals(object obj)
		{
			AsymmetricKeyEntry asymmetricKeyEntry = obj as AsymmetricKeyEntry;
			if (asymmetricKeyEntry == null)
			{
				return false;
			}
			return key.Equals(asymmetricKeyEntry.key);
		}

		public override int GetHashCode()
		{
			return ~key.GetHashCode();
		}
	}
}
