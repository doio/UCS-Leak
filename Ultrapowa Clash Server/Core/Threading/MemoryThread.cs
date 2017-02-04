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
                ManualResetEvent AllDone = new ManualResetEvent(false);

                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 5000;
                timer.Elapsed += ((ElapsedEventHandler)((s, a) =>
                {
                    AllDone.Reset();

                    /*foreach (Level _Player in ResourcesManager.GetInMemoryLevels())
                    {
                        if (!_Player.GetClient().IsClientSocketConnected())
                            ResourcesManager.DropClient(_Player.GetClient().GetSocketHandle());
                    }*/ // No more needed

                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();

                    AllDone.WaitOne();
                }));
                timer.Enabled = true;
            })).Start();
        }
    }
}
