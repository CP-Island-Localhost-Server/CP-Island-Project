using System;
using System.Runtime.InteropServices;
using Disney.LaunchPadFramework;

namespace Disney.MobileNetwork
{
	public class MemoryMonitorWindowsManager : MemoryMonitorManager
	{
		public static bool Enabled = true;

		[DllImport("MemoryMonitorWindows")]
		private static extern ulong _getProcessUsedBytes();

		protected override void Init()
		{
			try
			{
				GetProcessUsedBytes();
			}
			catch (Exception ex)
			{
				Enabled = false;
				Log.LogException(typeof(MemoryMonitorWindowsManager), ex);
			}
		}

		public override ulong GetProcessUsedBytes()
		{
			return Enabled ? _getProcessUsedBytes() : base.GetProcessUsedBytes();
		}
	}
}