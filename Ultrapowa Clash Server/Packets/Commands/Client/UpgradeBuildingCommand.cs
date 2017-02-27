using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 502
    internal class UpgradeBuildingCommand : Command
    {
        public int BuildingId { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        public UpgradeBuildingCommand(PacketReader br)
        {
            BuildingId = br.ReadInt32WithEndian();
            Unknown2 = br.ReadByte();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            if (go !=null)
            {
                var b = (ConstructionItem)go;
                if (b.CanUpgrade())
                {
                    var bd = b.GetConstructionItemData();
                    if (ca.HasEnoughResources(bd.GetBuildResource(b.GetUpgradeLevel() + 1),
                        bd.GetBuildCost(b.GetUpgradeLevel() + 1)))
                    {
                        if (level.HasFreeWorkers())
                        {
                            string name = level.GameObjectManager.GetGameObjectByID(BuildingId).GetData().GetName();
                            if (string.Equals(name, "Alliance Castle"))
                            {
                                ca.IncrementAllianceCastleLevel();
                                Building a = (Building)level.GameObjectManager.GetGameObjectByID(BuildingId);
                                BuildingData al = a.GetBuildingData();
                                ca.SetAllianceCastleTotalCapacity(al.GetUnitStorageCapacity(ca.GetAllianceCastleLevel()));
                            }
                            else if (string.Equals(name, "Town Hall"))
                                ca.IncrementTownHallLevel();

                            var rd = bd.GetBuildResource(b.GetUpgradeLevel() + 1);
                            ca.SetResourceCount(rd, ca.GetResourceCount(rd) - bd.GetBuildCost(b.GetUpgradeLevel() + 1));
                            b.StartUpgrading();
                        }
                    }
                }
            }
        }
    }
}
