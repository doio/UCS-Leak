using System.Collections.Generic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24344
    internal class BookmarkRemoveAllianceMessage : Message
    {
        public BookmarkRemoveAllianceMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24344);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.Add(1);
            Encrypt(data.ToArray());
        }
    }
}