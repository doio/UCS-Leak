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
            t.Interval = 5000;
            t.Elapsed += (s, a) =>
            {
                if (!r)
                {
                    r = true;

                    foreach (Client _Client in ResourcesManager.GetConnectedClients())
                    {
                        if (!_Client.IsClientSocketConnected())
                            ResourcesManager.DropClient(_Client.GetSocketHandle());
                    } // Removes disconnected Players from Memory.

                    r = false;
                }
            };
            t.Enabled = true;
        }
    }
}