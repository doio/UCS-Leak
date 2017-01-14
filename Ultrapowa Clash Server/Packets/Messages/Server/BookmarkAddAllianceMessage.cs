using System.Collections.Generic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24343
    internal class BookmarkAddAllianceMessage : Message
    {
        public BookmarkAddAllianceMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24343);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.Add(1);
            Encrypt(data.ToArray());
        }
    }
}