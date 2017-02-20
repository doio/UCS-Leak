using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;

namespace UCS.Core.Network
{
    class PacketProcessor
    {
        public static async void Receive(Message _Message)
        {
            _Message.Decrypt();
            _Message.Decode();
            _Message.Process(_Message.Client.GetLevel());
        }               

        public static async void Send(Message _Message)
        {
            try
            {
                _Message.Encode();
                if (_Message.GetMessageType() == 20000)
                {
                    byte[] sessionKey = ((RC4SessionKey)_Message).Key;
                    _Message.Client.UpdateKey(sessionKey);
                }
                _Message.Process(_Message.Client.GetLevel());
                _Message.Client.Socket.BeginSend(_Message.GetRawData(), 0, _Message.GetRawData().Length, SocketFlags.None, null, null);
            }
            catch (Exception)
            {
            }
        }
    }
}
