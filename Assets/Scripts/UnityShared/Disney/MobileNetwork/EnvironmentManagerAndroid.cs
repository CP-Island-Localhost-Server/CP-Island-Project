using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class EnvironmentManagerAndroid : EnvironmentManager
	{
#if UNITY_ANDROID
		private AndroidJavaObject m_androidActivity = null;

		private AndroidJavaObject m_androidContext = null;

		private AndroidJavaObject m_androidEnvironmentManager = null;

		public AndroidJavaObject AndroidActivity
		{
			get
			{
				return m_androidActivity;
			}
		}

		public AndroidJavaObject AndroidContext
		{
			get
			{
				return m_androidContext;
			}
		}

		protected override string _SKU
		{
			get
			{
				if (string.IsNullOrEmpty(EnvironmentManager.mSKU))
				{
					EnvironmentManager.mSKU = m_androidEnvironmentManager.Call<string>("getSKU", new object[0]);
				}
				return EnvironmentManager.mSKU;
			}
		}

		protected override string _SKUPluginManifestJson
		{
			get
			{
				return m_androidEnvironmentManager.Call<string>("getSKUPluginManifestJson", new object[0]);
			}
		}

		public override void ShowAlert(ShowAlertDelegate showAlertDelegate, string title, string message, string viewButtonText, string cancelButtonText)
		{
			base.ShowAlert(showAlertDelegate, title, message, viewButtonText, cancelButtonText);
			m_androidEnvironmentManager.Call("showAlert", title, message, viewButtonText, cancelButtonText);
		}

		protected override void _Init()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				m_androidActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				m_androidContext = m_androidActivity.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			}
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.disney.mobilenetwork.utils.EnvironmentManager"))
			{
				if (androidJavaClass2 != null)
				{
					m_androidEnvironmentManager = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
					m_androidEnvironmentManager.Call("listenForHeadphonesConnected");
				}
			}
			EnvironmentManager.mBundleIdentifier = _GetBundleIdentifier();
			EnvironmentManager.mBundleVersionCode = _GetBundleVersionString();
			string text = EnvironmentManager.NormalizeVersionString(EnvironmentManager.mBundleVersionCode);
			if (!string.IsNullOrEmpty(text))
			{
				EnvironmentManager.mBundleVersion = new Version(text);
			}
			else
			{
				EnvironmentManager.mBundleVersion = new Version(0, 0, 0);
			}
			BuildSettings.LoadSettings();
		}

		private string _GetBundleIdentifier()
		{
			string result = null;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<string>("getBundleIdentifier", new object[0]);
			}
			return result;
		}

		private string _GetBundleVersionString()
		{
			string result = null;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<string>("getBundleVersion", new object[0]);
			}
			return result;
		}

		protected override string _GetLocale()
		{
			string result = null;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<string>("getLocale", new object[0]);
			}
			return result;
		}

		protected override string _GetDeviceLanguage()
		{
			string result = null;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<string>("getDeviceLanguage", new object[0]);
			}
			return result;
		}

		protected override int _GetDiskSpaceFreeMegabytes()
		{
			int result = 0;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<int>("getDiskSpaceFreeMegabytes", new object[0]);
			}
			return result;
		}

		protected override bool _GetIsMusicPlaying()
		{
			bool result = false;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<bool>("getIsMusicPlaying", new object[0]);
			}
			return result;
		}

		protected override bool _GetAreHeadphonesConnected()
		{
			bool result = false;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<bool>("getAreHeadphonesConnected", new object[0]);
			}
			return result;
		}

		protected override bool _GetIsExternalLinksRestricted()
		{
			bool result = false;
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<bool>("getIsExternalLinksRestricted", new object[0]);
			}
			return result;
		}

		protected override string _GetBuildSettingsJson()
		{
			string result = "";
			if (m_androidEnvironmentManager != null)
			{
				result = m_androidEnvironmentManager.Call<string>("getBuildSettingsJson", new object[0]);
			}
			return result;
		}

		protected override void _ShowStatusBar(bool show, bool useLightColor)
		{
			m_androidEnvironmentManager.Call("showStatusBar", show);
		}
#endif
	}

}
