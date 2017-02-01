using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS.Logic;
using UCS.Packets;

namespace UCS.Core
{
    internal class MessageManager
    {
        private static BlockingCollection<Message> m_vPackets = new BlockingCollection<Message>();

        public MessageManager()
        {
            PacketProcessingDelegate packetProcessing = PacketProcessing;
            packetProcessing.BeginInvoke(null, null);
        }

        public static void ProcessPacket(Message p)
        {
            m_vPackets.Add(p);
        }

        private void PacketProcessing()
        {
            while (true)
            {
                Message packet = m_vPackets.Take();
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Message p = (Message)state;
                    Level pl = p.Client.GetLevel();
                    try
                    {
                        p.Decode();
                        p.Process(pl);
                    }
                    catch (Exception e)
                    {
                    }
                }, packet);
            }
        }

        private delegate void PacketProcessingDelegate();
    }
}
