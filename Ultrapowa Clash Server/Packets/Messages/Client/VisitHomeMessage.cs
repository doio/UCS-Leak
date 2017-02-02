using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14113
    internal class VisitHomeMessage : Message
    {
        public VisitHomeMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public long AvatarId { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                AvatarId = br.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
        {
            Level targetLevel = ResourcesManager.GetPlayer(AvatarId);
            targetLevel.Tick();
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            PacketManager.Send(new VisitedHomeDataMessage(Client, targetLevel, level));
            if (alliance != null)
            {
                PacketManager.Send(new AllianceStreamMessage(Client, alliance));
            }
        }
    }
}
