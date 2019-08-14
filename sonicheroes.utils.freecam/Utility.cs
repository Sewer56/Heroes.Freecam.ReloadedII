using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace sonicheroes.utils.freecam
{
    public static class Utility
    {
        private static Process _currentProcess = Process.GetCurrentProcess();

        /// <summary>
        /// Returns true if the window or any subwindows are activated.
        /// </summary>
        public static bool IsWindowActivated()
        {
            IntPtr activatedHandle = GetForegroundWindow();

            if (activatedHandle == IntPtr.Zero)
                return false;

            GetWindowThreadProcessId(activatedHandle, out int activeProcessId);
            return activeProcessId == _currentProcess.Id;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
    }
}
