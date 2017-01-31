using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core.Settings;
using UCS.Packets;
using Timer = System.Timers.Timer;

namespace UCS.Core.Threading
{
    class MemoryThread
    {
        private static Thread T { get; set; }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize,UIntPtr maximumWorkingSetSize);

        public MemoryThread()
        {
            new Thread(() =>
            {
                Timer t = new Timer();
                t.Interval = 5000;
                t.Elapsed += (s, a) =>
                {
                    foreach (var p in ResourcesManager.GetInMemoryLevels())
                    {
                        if (!p.GetClient().IsClientSocketConnected())
                            ResourcesManager.DropClient(p.GetClient().GetSocketHandle());
                    }

                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
                };
                t.Enabled = true;
            }).Start();
        }
    }
}