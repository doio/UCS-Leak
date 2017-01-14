using System.Collections.Generic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24317
    internal class AnswerJoinRequestAllianceMessage : Message
    {
        public AnswerJoinRequestAllianceMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24317);
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            Encrypt(pack.ToArray());
        }
    }
}