using System.Collections.Generic;
using UCS.Packets.Messages.Client;

namespace UCS.Packets.Messages.Server
{
    // Packet 20108
    internal class KeepAliveOkMessage : Message
    {
        public KeepAliveOkMessage(Packets.Client client, KeepAliveMessage cka) : base(client)
        {
            SetMessageType(20108);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            Encrypt(data.ToArray());
        }
    }
}