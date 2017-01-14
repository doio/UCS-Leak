using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 531
    internal class CancelHeroUpgradeCommand : Command
    {
        readonly int m_vBuildingId;

        public CancelHeroUpgradeCommand(PacketReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
            {
                if (go.ClassId == 0)
                {
                    var b = (Building) go;
                    var hbc = b.GetHeroBaseComponent();
                    if (hbc != null)
                    {
                        hbc.CancelUpgrade();
                    }
                }
            }
        }
    }
}