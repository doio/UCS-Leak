using System.Diagnostics;
using Updater.Core.Threading;

namespace Updater.Core.Checker
{
    class ProcessChecker
    {
        public static void Check()
        {
            Process[] UcsOpen = Process.GetProcessesByName("UCS.exe");
            if (UcsOpen.Length != 0)
            {
                foreach (var Process in Process.GetProcessesByName("UCS.exe"))
                {
                    Process.Kill();
                }
                UpdateThread.Start();
            }
            else if (UcsOpen.Length == 0)
            {
                UpdateThread.Start();
            }
        }
    }
}
