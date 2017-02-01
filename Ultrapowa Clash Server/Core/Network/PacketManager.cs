using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;

namespace UCS.Core.Network
{
    internal static class PacketManager
    {
        public static void Receive(Message p)
        {
            p.Decrypt();
            p.Decode();
            p.Process(p.Client.GetLevel());
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
                p.Client.Socket.Send(p.GetRawData());
            }
            catch (Exception)
            {
            }
        }
    }
}
