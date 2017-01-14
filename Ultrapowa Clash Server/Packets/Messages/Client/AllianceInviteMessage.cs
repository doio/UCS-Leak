using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 14322
    internal class AllianceInviteMessage : Message
    {
        public AllianceInviteMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
        }
    }
}