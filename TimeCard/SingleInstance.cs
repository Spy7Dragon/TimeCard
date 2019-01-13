using System;
using System.Threading;

namespace TimeCard
{
    /// <summary>
    /// 
    /// </summary>
    public static class SingleInstance
    {
        public static readonly int WM_SHOWFIRSTINSTANCE =
            WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ProgramInfo.AssemblyGuid);
        private static Mutex s_Mutex;

        public static bool Start()
        {
            bool only_instance;
            string mutex_name = string.Format("Local\\{0}", ProgramInfo.AssemblyGuid);

            s_Mutex = new Mutex(true, mutex_name, out only_instance);
            return only_instance;
        }

        public static void ShowFirstInstance()
        {
            WinApi.PostMessage(
                (IntPtr)WinApi.HWND_BROADCAST,
                WM_SHOWFIRSTINSTANCE,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        public static void Stop()
        {
            s_Mutex.ReleaseMutex();
        }
    }
}