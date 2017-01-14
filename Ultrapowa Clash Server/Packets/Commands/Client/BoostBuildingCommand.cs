using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 526
    internal class BoostBuildingCommand : Command
    {
        public BoostBuildingCommand(PacketReader br)
        {
            BuildingIds = new List<int>();
            BoostedBuildingsCount = br.ReadInt32WithEndian();
            for (int i = 0; i < BoostedBuildingsCount; i++)
            {
                BuildingIds.Add(br.ReadInt32WithEndian());
            }
        }

        public override void Execute(Level level)
        {
            ClientAvatar ca = level.GetPlayerAvatar();

            foreach(int buildingId in BuildingIds)
            {
                GameObject go = level.GameObjectManager.GetGameObjectByID(buildingId);
                ConstructionItem b = (ConstructionItem)go;
                int costs = ((BuildingData)b.GetConstructionItemData()).BoostCost[b.UpgradeLevel];
                if (ca.HasEnoughDiamonds(costs))
                {
                    b.BoostBuilding();
                    ca.SetDiamonds(ca.GetDiamonds() - costs);
                }
            }
        }

        public int BoostedBuildingsCount { get; set; }
        public List<int> BuildingIds { get; set; }
    }
}