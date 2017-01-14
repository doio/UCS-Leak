using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14331
    internal class AskForAllianceWarDataMessage : Message
    {
        public AskForAllianceWarDataMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
            }
        }

        public override void Process(Level level)
        {
            new AllianceWarDataMessage(Client).Send();
        }
    }
}