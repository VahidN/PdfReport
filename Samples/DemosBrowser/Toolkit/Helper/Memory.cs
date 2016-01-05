using System;
using System.Diagnostics;

namespace DemosBrowser.Toolkit.Helper
{
    public static class Memory
    {
        public static void ReEvaluatedWorkingSet()
        {
            try
            {
                var loProcess = Process.GetCurrentProcess();
                loProcess.MaxWorkingSet = (IntPtr)((int)loProcess.MaxWorkingSet + 1);
            }
            catch
            { }
        }
    }
}
