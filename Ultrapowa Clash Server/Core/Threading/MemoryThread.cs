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
        static bool r = false;

        public MemoryThread()
        {
            Timer t = new Timer();
            t.Interval = 800;
            t.Elapsed += (s, a) =>
            {
                if (!r)
                {
                    r = true;

                    foreach (Client p in ResourcesManager.GetConnectedClients())
                    {
                        if (!p.IsClientSocketConnected())
                            ResourcesManager.DropClient(p.GetSocketHandle());
                    }

                    r = false;
                }
            };
            t.Enabled = true;
        }
    }
}