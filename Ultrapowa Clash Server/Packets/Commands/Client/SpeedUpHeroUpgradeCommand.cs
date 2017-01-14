using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 528
    internal class SpeedUpHeroUpgradeCommand : Command
    {
        public SpeedUpHeroUpgradeCommand(PacketReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            m_vUnknown1 = br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);

            if (go != null)
            {
                var b = (Building) go;
                var hbc = b.GetHeroBaseComponent();
                if (hbc != null)
                    hbc.SpeedUpUpgrade();
            }
        }

        readonly int m_vBuildingId;
        int m_vUnknown1;
    }
}