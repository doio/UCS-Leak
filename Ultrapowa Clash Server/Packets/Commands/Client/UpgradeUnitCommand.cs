using System.IO;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 516
    internal class UpgradeUnitCommand : Command
    {
        public UpgradeUnitCommand(PacketReader br)
        {
            BuildingId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
            UnitData = (CombatItemData) br.ReadDataReference();
            Unknown2 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            var b = (Building) go;
            var uuc = b.GetUnitUpgradeComponent();
            var unitLevel = ca.GetUnitUpgradeLevel(UnitData);
            if (uuc.CanStartUpgrading(UnitData))
            {
                var cost = UnitData.GetUpgradeCost(unitLevel);
                var rd = UnitData.GetUpgradeResource(unitLevel);
                if (ca.HasEnoughResources(rd, cost))
                {
                    ca.SetResourceCount(rd, ca.GetResourceCount(rd) - cost);
                    uuc.StartUpgrading(UnitData);
                }
            }
        }

        public int BuildingId { get; set; }
        public CombatItemData UnitData { get; set; }
        public uint Unknown1 { get; set; } 
        public uint Unknown2 { get; set; }
    }
}