using System;
using System.IO;
using UnityEngine;

namespace Disney.MobileNetwork
{
    public class MemoryMonitorLinuxManager : MemoryMonitorManager
    {
#if UNITY_STANDALONE_LINUX
        protected override void Init()
        {
            Debug.Log("Memory monitoring initialized for Linux.");
        }

        private ulong GetMemoryInfo(string label)
        {
            try
            {
                using (StreamReader sr = new StreamReader("/proc/meminfo"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith(label))
                        {
                            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            return ulong.Parse(parts[1]) * 1024; // Convert from kB to bytes
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading memory info: {ex.Message}");
            }

            return 0;
        }

        public override ulong GetHeapSize()
        {
            return GetMemoryInfo("MemTotal:");
        }

        public override ulong GetFreeBytes()
        {
            return GetMemoryInfo("MemFree:");
        }

        public override ulong GetTotalBytes()
        {
            return GetHeapSize();
        }

        public override float GetBatteryPercent()
        {
            Debug.LogWarning("Battery monitoring is not implemented for Linux.");
            return 1f;
        }

        public override ulong GetProcessUsedBytes()
        {
            try
            {
                string processMemoryFile = $"/proc/{System.Diagnostics.Process.GetCurrentProcess().Id}/status";
                using (StreamReader sr = new StreamReader(processMemoryFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("VmRSS:"))
                        {
                            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            return ulong.Parse(parts[1]) * 1024; // Convert from kB to bytes
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading process memory info: {ex.Message}");
            }

            return 0;
        }
#endif
    }
}
