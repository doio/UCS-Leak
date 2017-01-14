using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core.Settings;
using UCS.Logic;
using Timer = System.Timers.Timer;

namespace UCS.Core.Threading
{
    class MemoryThread
    {    
        public MemoryThread()
        {
            new Thread(() =>
            {
                Timer t = new Timer();
                t.Interval = 5000;
                t.Elapsed += (s, a) =>
                {
                    foreach (Level p in ResourcesManager.GetInMemoryLevels())
                    {
                        if (!p.GetClient().IsClientSocketConnected())
                            ResourcesManager.DropClient(p.GetClient().GetSocketHandle());
                    }

                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();
                };
                t.Enabled = true;
            }).Start();
        }
    }
}