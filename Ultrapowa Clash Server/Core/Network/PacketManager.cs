using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;

namespace UCS.Core.Network
{
    internal class PacketManager
    {
        private static readonly BlockingCollection<Message> m_vIncomingPackets = new BlockingCollection<Message>();
        private static readonly BlockingCollection<Message> m_vOutgoingPackets = new BlockingCollection<Message>();

        public PacketManager()
        {
            IncomingProcessingDelegate incomingProcessing = IncomingProcessing;
            OutgoingProcessingDelegate outgoingProcessing = OutgoingProcessing;

            incomingProcessing.BeginInvoke(null, null);
            outgoingProcessing.BeginInvoke(null, null);
        }

        public static void ProcessIncomingPacket(Message p)
        {
            m_vIncomingPackets.Add(p);
        }

        public static void Send(Message p)
        {
            try
            {
                p.Encode();
                if (p.GetMessageType() == 20000)
                {
                    byte[] sessionKey = ((RC4SessionKey)p).Key;
                    p.Client.UpdateKey(sessionKey);
                }
                p.Process(p.Client.GetLevel());
                m_vOutgoingPackets.Add(p);
            }
            catch (Exception)
            {
            }
        }

        private static void IncomingProcessing()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Message p = m_vIncomingPackets.Take();
                    try
                    {
                        p.Decrypt();
                        p.Decode();
                        MessageManager.ProcessPacket(p);
                    }
                    catch (Exception)
                    {
                    }
                }
            }).Start();
        }

        private static void OutgoingProcessing()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Message p = m_vOutgoingPackets.Take();
                    try
                    {
                        p.Client.Socket.BeginSend(p.GetRawData(), 0, p.GetRawData().Length, 0, null, p.Client.Socket);
                    }
                    catch (Exception)
                    {
                    }
                }
            }).Start();
        }

        private delegate void IncomingProcessingDelegate();

        private delegate void OutgoingProcessingDelegate();
    }
}