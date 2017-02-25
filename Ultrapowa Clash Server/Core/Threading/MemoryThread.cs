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
                _Timer = new System.Timers.Timer();
                _Timer.Interval = Constants.CleanInterval;
                _Timer.Elapsed += ((s, a) => Clean());
                _Timer.Start();
            });

            _Thread.Priority = ThreadPriority.Lowest;

            _Thread.Start();
        }

        public static void Clean()
        {
            try
            {
                foreach (Level _Player in ResourcesManager.GetInMemoryLevels())
                {
                    if (!_Player.GetClient().IsClientSocketConnected())
                    {
                        _Player.GetClient().Socket.Close();
                        ResourcesManager.DropClient(_Player.GetClient().GetSocketHandle());
                    }
                }

                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();
            } catch (Exception) { }
        }

        public void Dispose()
        {
            _Timer.Stop();
            _Thread.Abort();
        }
    }
}
