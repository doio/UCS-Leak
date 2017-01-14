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
            T = new Thread(() =>
            {
                new ResourcesManager();           
                new CSVManager();
                new ObjectManager();
                new Gateway();
            }); T.Start();
        }

        public static void Stop()
        {
            T.Abort();
        }
    }
}
