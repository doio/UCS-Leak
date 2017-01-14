using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 517
    internal class SpeedUpUpgradeUnitCommand : Command
    {
        readonly int m_vBuildingId;

        public SpeedUpUpgradeUnitCommand(PacketReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
            {
                if (go.ClassId == 0)
                {
                    var b = (Building) go;
                    var uuc = b.GetUnitUpgradeComponent();
                    if (uuc != null)
                    {
                        if (uuc.GetCurrentlyUpgradedUnit() != null)
                        {
                            uuc.SpeedUp();
                        }
                    }
                }
            }
        }
    }
}