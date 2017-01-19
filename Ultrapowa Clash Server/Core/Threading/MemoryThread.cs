using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core.Settings;
using UCS.Packets;
using Timer = System.Timers.Timer;

namespace UCS.Core.Threading
{
    class MemoryThread
    {    
        public MemoryThread()
        {
            new Thread(() =>
            {
                Thread.Sleep(6000);

                Timer t = new Timer();
                bool Running = false;
                t.Interval = 5000;
                t.Elapsed += (s, a) =>
                {
                    if (!Running)
                    {
                        Running = true;

                        foreach (Client p in ResourcesManager.GetConnectedClients())
                        {
                            if (!p.IsClientSocketConnected())
                                ResourcesManager.DropClient(p.GetSocketHandle());
                        }

                        GC.Collect(GC.MaxGeneration);
                        GC.WaitForPendingFinalizers();

                        Running = false;
                    }
                };
                t.Enabled = true;
            }).Start();
        }
    }
}