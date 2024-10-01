using Disney.MobileNetwork;
using UnityEngine;

namespace Disney.MobileNetwork
{
    public class MemoryMonitorAndroidManager : MemoryMonitorManager
    {
#if UNITY_ANDROID
        private AndroidJavaObject androidPlugin = null;

        protected override void Init()
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.mobilenetwork.plugins.MemoryMonitorPlugin"))
            {
                if (androidJavaClass != null)
                {
                    androidPlugin = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
                }
            }
        }

        public override ulong GetHeapSize()
        {
            if (androidPlugin != null)
            {
                int num = androidPlugin.Call<int>("GetHeapSize", new object[0]);
                return (ulong)((long)num * 1024L * 1024);
            }
            else
            {
                Debug.LogError("Android plugin not initialized.");
                return 0;
            }
        }

        public override ulong GetFreeBytes()
        {
            if (androidPlugin != null)
            {
                return (ulong)androidPlugin.Call<long>("GetFreeBytes", new object[0]);
            }
            else
            {
                Debug.LogError("Android plugin not initialized.");
                return 0;
            }
        }

        public override ulong GetTotalBytes()
        {
            if (androidPlugin != null)
            {
                return (ulong)androidPlugin.Call<long>("GetTotalBytes", new object[0]);
            }
            else
            {
                Debug.LogError("Android plugin not initialized.");
                return 0;
            }
        }

        public override float GetBatteryPercent()
        {
            if (androidPlugin != null)
            {
                return androidPlugin.Call<float>("GetBatteryPercent", new object[0]);
            }
            else
            {
                Debug.LogError("Android plugin not initialized.");
                return 0f;
            }
        }

        public override ulong GetProcessUsedBytes()
        {
            if (androidPlugin != null)
            {
                return (ulong)androidPlugin.Call<long>("GetProcessUsedBytes", new object[0]);
            }
            else
            {
                Debug.LogError("Android plugin not initialized.");
                return 0;
            }
        }
#endif
    }
}
