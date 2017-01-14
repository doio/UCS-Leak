using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    // Packet 24104
    internal class OutOfSyncMessage : Message
    {
        public OutOfSyncMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24104);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            Encrypt(data.ToArray());
        }
    }
}
