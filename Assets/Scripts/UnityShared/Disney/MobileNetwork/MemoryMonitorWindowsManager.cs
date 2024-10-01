using System;
using UnityEngine;

namespace Disney.MobileNetwork
{
    public class MemoryMonitorWindowsManager : MemoryMonitorManager
    {
#if UNITY_STANDALONE_WIN
        public static bool Enabled = false;  // Disable this since the DLL is missing

        // Disable the DLL import and any related functionality
        protected override void Init()
        {
            Debug.LogWarning("Memory monitoring disabled for Windows: MemoryMonitorWindows.dll not found.");
        }

        public override ulong GetProcessUsedBytes()
        {
            return 0;  // Return a default value
        }
#else
        // Other platforms (if DLL is used elsewhere)
        public static bool Enabled = true;

        public override ulong GetProcessUsedBytes()
        {
            // Other platform-specific code if needed
            return base.GetProcessUsedBytes();
        }
#endif
    }
}
