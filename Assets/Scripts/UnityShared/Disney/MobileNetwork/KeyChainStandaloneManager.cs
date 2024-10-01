using UnityEngine;

namespace Disney.MobileNetwork
{
	public class KeyChainStandaloneManager : KeyChainManager
	{
		protected override void Init()
		{
		}

		public override void PutString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public override string GetString(string key)
		{
			return PlayerPrefs.GetString(key, "");
		}

		public override void RemoveString(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}
	}
}
