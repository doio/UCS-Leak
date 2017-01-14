using System.IO;
using UCS.Helpers;

namespace UCS.Packets.Commands.Client
{
    // Packet 530
    internal class SpeedUpHeroHealthCommand : Command
    {
        //int m_vBuildingId;

        public SpeedUpHeroHealthCommand(PacketReader br)
        {
            /*
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
            */
        }
    }
}