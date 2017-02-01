using System;
using System.Configuration;
using System.Threading;
using UCS.Core.Network;

namespace UCS.Core.Threading
{
    class NetworkThread
    {
        static Thread T;

        static NetworkThread()
        {
            new Thread(() =>
            {
                new PacketManager();
                new MessageManager();
                new ResourcesManager();
                new CSVManager();
                new ObjectManager();
                new Gateway();
            }).Start();
        }

        public static void Stop()
        {
            T.Abort();
        }
    }
}
