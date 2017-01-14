using System.Collections.Generic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24303
    internal class AllianceJoinOkMessage : Message
    {
        public AllianceJoinOkMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24303);
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            Encrypt(pack.ToArray());
        }
    }
}