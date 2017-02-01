using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core.Settings;
using UCS.Logic;
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
                bool r = false;
                Timer t = new Timer();
                t.Interval = 60000;
                t.Elapsed += (s, a) =>
                {
                    if (!r)
                    {
                        r = true;

                        /*foreach (Level p in ResourcesManager.GetInMemoryLevels())
                        {
                            if (!p.GetClient().IsClientSocketConnected())
                                ResourcesManager.DropClient(p.GetClient().GetSocketHandle());
                        }*/

                        GC.Collect(GC.MaxGeneration);
                        //GC.WaitForPendingFinalizers();
                        //SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)long.MaxValue);

                        r = false;
                    }
                };
                t.Enabled = true;
            }).Start();
        }
    }
}