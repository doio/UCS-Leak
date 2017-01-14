using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 520
    internal class UnlockBuildingCommand : Command
    {
        public UnlockBuildingCommand(PacketReader br)
        {
            BuildingId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);

            var b = (ConstructionItem) go;

            var bd = (BuildingData) b.GetConstructionItemData();

            if (ca.HasEnoughResources(bd.GetBuildResource(b.GetUpgradeLevel()), bd.GetBuildCost(b.GetUpgradeLevel())))
            {
                string name = level.GameObjectManager.GetGameObjectByID(BuildingId).GetData().GetName();
                Logger.Write("Unlocking Building: " + name + " (" + BuildingId + ')');
                if (string.Equals(name, "Alliance Castle"))
                {
                    ca.IncrementAllianceCastleLevel();
                    Building a = (Building)level.GameObjectManager.GetGameObjectByID(BuildingId);
                    BuildingData al = a.GetBuildingData();
                    ca.SetAllianceCastleTotalCapacity(al.GetUnitStorageCapacity(ca.GetAllianceCastleLevel()));
                }
                var rd = bd.GetBuildResource(b.GetUpgradeLevel());
                ca.SetResourceCount(rd, ca.GetResourceCount(rd) - bd.GetBuildCost(b.GetUpgradeLevel()));
                b.Unlock();
            }
        }
        public int BuildingId { get; set; }
        public uint Unknown1 { get; set; }
    }
}