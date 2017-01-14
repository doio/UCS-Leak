using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14325
    internal class AskForAvatarProfileMessage : Message
    {
        public AskForAvatarProfileMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        long m_vAvatarId;
        long m_vCurrentHomeId;

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                m_vAvatarId = br.ReadInt64();
                br.ReadByte();
                m_vCurrentHomeId = br.ReadInt64();
            }
        }

        public override void Process(Level level)
        {
            Level targetLevel = ResourcesManager.GetPlayer(m_vAvatarId);
            if (targetLevel != null)
            {
                targetLevel.Tick();
                AvatarProfileMessage p = new AvatarProfileMessage(Client);
                p.SetLevel(targetLevel);
                p.Send();
            }
        }
    }
}
