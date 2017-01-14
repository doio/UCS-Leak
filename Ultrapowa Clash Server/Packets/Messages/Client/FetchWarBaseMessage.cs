using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet ?
    internal class FetchWarBaseMessage : Message
    {
        public FetchWarBaseMessage(Packets.Client client, PacketReader br) : base(client, br)
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