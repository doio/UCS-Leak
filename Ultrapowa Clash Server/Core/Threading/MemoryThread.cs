using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UCS.Core.Settings;
using UCS.Logic;
using UCS.Packets;

namespace UCS.Core.Threading
{
    internal class MemoryThread
    {
        public MemoryThread()
        {
            new Thread((ThreadStart)(() =>
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 5000.0;
                timer.Elapsed += (ElapsedEventHandler)((s, a) =>
                {
                    foreach (Level inMemoryLevel in ResourcesManager.GetInMemoryLevels())
                    {
                        if (!inMemoryLevel.GetClient().IsClientSocketConnected())
                            ResourcesManager.DropClient(inMemoryLevel.GetClient().GetSocketHandle());
                    }
                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();
                });
                timer.Enabled = true;
            })).Start();
        }
    }
}
