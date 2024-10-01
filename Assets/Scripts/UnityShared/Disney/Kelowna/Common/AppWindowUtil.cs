using System;
using System.Runtime.InteropServices;

namespace Disney.Kelowna.Common
{
    public static class AppWindowUtil
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static void SetTitle(string title)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            SetWindowText(foregroundWindow, title);
        }

        public static void StartCustomWindowManager()
        {
        }
    }
}