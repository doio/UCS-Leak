using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Commands.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14305
    internal class JoinAllianceMessage : Message
    {
        long m_vAllianceId;

        public JoinAllianceMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                m_vAllianceId = br.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
        {
            Alliance alliance = ObjectManager.GetAlliance(m_vAllianceId);
            if (alliance != null)
            {
                if (!alliance.IsAllianceFull())
                {
                    level.GetPlayerAvatar().SetAllianceId(alliance.GetAllianceId());
                    AllianceMemberEntry member = new AllianceMemberEntry(level.GetPlayerAvatar().GetId());
                    member.SetRole(1);
                    alliance.AddAllianceMember(member);

                    JoinedAllianceCommand b = new JoinedAllianceCommand();
                    b.SetAlliance(alliance);

                    AllianceRoleUpdateCommand c = new AllianceRoleUpdateCommand();
                    c.SetAlliance(alliance);
                    c.SetRole(1);
                    c.Tick(level);

                    AvailableServerCommandMessage a = new AvailableServerCommandMessage(Client);
                    a.SetCommandId(1);
                    a.SetCommand(b);

                    AvailableServerCommandMessage d = new AvailableServerCommandMessage(Client);
                    d.SetCommandId(8);
                    d.SetCommand(c);

                    PacketProcessor.Send(a);
                    PacketProcessor.Send(d);

                    PacketProcessor.Send(new AllianceStreamMessage(Client, alliance));
                }
            }
        }
    }
}
