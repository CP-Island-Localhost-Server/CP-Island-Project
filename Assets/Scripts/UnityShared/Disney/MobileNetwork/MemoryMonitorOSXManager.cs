using Disney.MobileNetwork;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Disney.MobileNetwork
{
    public class MemoryMonitorOSXManager : MemoryMonitorManager
    {
#if UNITY_STANDALONE_OSX
        // External methods to get memory and battery data on macOS
        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetHeapSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _GetHeapSize();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetFreeBytes", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _GetFreeBytes();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetTotalBytes", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _GetTotalBytes();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetBatteryPercent", CallingConvention = CallingConvention.Cdecl)]
        private static extern float _GetBatteryPercent();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetProcessUsedBytes", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _GetProcessUsedBytes();

        protected override void Init()
        {
            try
            {
                // Check if running on Apple Silicon or Intel
                if (SystemInfo.processorType.Contains("Apple"))
                {
                    Debug.Log("Running on Apple Silicon (ARM64).");
                }
                else
                {
                    Debug.Log("Running on Intel-based macOS (x86_64).");
                }

                // Call the function to get memory usage
                GetProcessUsedBytes();
            }
            catch (DllNotFoundException dllEx)
            {
                Debug.LogError($"MemoryMonitorOSX.dylib not found: {dllEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public override ulong GetHeapSize()
        {
            return _GetHeapSize();
        }

        public override ulong GetFreeBytes()
        {
            return _GetFreeBytes();
        }

        public override ulong GetTotalBytes()
        {
            return _GetTotalBytes();
        }

        public override float GetBatteryPercent()
        {
            return _GetBatteryPercent();
        }

        public override ulong GetProcessUsedBytes()
        {
            return _GetProcessUsedBytes();
        }
#endif
    }
}
