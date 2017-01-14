using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14401
    internal class TopGlobalAlliancesMessage : Message
    {
        public int unknown { get; set; }
        public TopGlobalAlliancesMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                unknown = GetData().Length == 10 ? GetData()[9] : br.Read();
            }
        }

        public override void Process(Level level)
        {
            if (unknown == 0)
                new GlobalAlliancesMessage(Client).Send();
            else
                new LocalAlliancesMessage(Client).Send();
        }
    }
}
