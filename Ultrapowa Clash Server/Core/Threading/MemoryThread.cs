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
    internal class MemoryThread : IDisposable
    {
        private System.Timers.Timer _Timer = null;
        private Thread _Thread             = null;

        public MemoryThread()
        {
            _Thread = new Thread(() =>
            {
                ManualResetEvent AllDone = new ManualResetEvent(false);

                _Timer = new System.Timers.Timer();
                _Timer.Interval = 2000;
                _Timer.Elapsed += (((s, a) =>
                {
                    AllDone.Reset();

                    foreach (Level _Player in ResourcesManager.GetInMemoryLevels())
                    {
                        if (!_Player.GetClient().IsClientSocketConnected())
                            ResourcesManager.DropClient(_Player.GetClient().GetSocketHandle());
                    }

                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();

                    AllDone.WaitOne();
                }));
                _Timer.Enabled = true;
            });

            _Thread.Start();
        }

        public void Dispose()
        {
            _Timer.Stop();
        }
    }
}
