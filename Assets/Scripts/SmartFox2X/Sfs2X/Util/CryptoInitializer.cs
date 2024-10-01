using System;
using System.Collections;
using Sfs2X.Core;
using UnityEngine;

namespace Sfs2X.Util
{
	public class CryptoInitializer
	{
		private const string KEY_SESSION_TOKEN = "SessToken";

		private const string TARGET_SERVLET = "/BlueBox/CryptoManager";

		private SmartFox sfs;

		private bool useHttps = true;

		public CryptoInitializer(SmartFox sfs)
		{
			if (!sfs.IsConnected)
			{
				throw new InvalidOperationException("Cryptography cannot be initialized before connecting to SmartFoxServer!");
			}
			if (sfs.GetSocketEngine().CryptoKey != null)
			{
				throw new InvalidOperationException("Cryptography is already initialized!");
			}
			this.sfs = sfs;
		}

		public IEnumerator Run()
		{
			string targetUrl = ((!useHttps) ? "http://" : "https://") + sfs.Config.Host + ":" + ((!useHttps) ? sfs.Config.HttpPort : sfs.Config.HttpsPort) + "/BlueBox/CryptoManager";
			WWWForm vars = new WWWForm();
			vars.AddField("SessToken", sfs.SessionToken);
			sfs.Logger.Debug("Attempting encryption initialization");
			WWW www = new WWW(targetUrl, vars);
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				OnHttpResponse(www.text);
			}
			else
			{
				OnHttpError(www.error);
			}
		}

		private void OnHttpResponse(string rawData)
		{
			byte[] data = Convert.FromBase64String(rawData);
			ByteArray byteArray = new ByteArray();
			ByteArray byteArray2 = new ByteArray();
			byteArray.WriteBytes(data, 0, 16);
			byteArray2.WriteBytes(data, 16, 16);
			sfs.GetSocketEngine().CryptoKey = new CryptoKey(byteArray2, byteArray);
			Hashtable hashtable = new Hashtable();
			hashtable["success"] = true;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.CRYPTO_INIT, hashtable));
		}

		private void OnHttpError(string errorMsg)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["success"] = false;
			hashtable["errorMessage"] = errorMsg;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.CRYPTO_INIT, hashtable));
		}
	}
}
