using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14336
    internal class AskForAllianceWarHistoryMessage : Message
    {
        public AskForAllianceWarHistoryMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        static long AllianceID { get; set; }
        static long WarID { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                AllianceID = br.ReadInt64();
                WarID = br.ReadInt64();
            }
        }

        public override void Process(Level level)
        {
            Alliance all = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            new AllianceWarHistoryMessage(Client, all).Send();
        }
    }
}