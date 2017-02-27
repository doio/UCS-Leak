using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 549
    internal class UpgradeMultipleBuildingsCommand : Command
    {
        public UpgradeMultipleBuildingsCommand(PacketReader br)
        {
            m_vIsAltResource = br.ReadByte();
            m_vBuildingIdList = new List<int>();
            var buildingCount = br.ReadInt32WithEndian();
            for (var i = 0; i < buildingCount; i++)
            {
                var buildingId = br.ReadInt32WithEndian();
                m_vBuildingIdList.Add(buildingId);
            }
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            foreach (var buildingId in m_vBuildingIdList)
            {
                var b = (Building) level.GameObjectManager.GetGameObjectByID(buildingId);
                if (b.CanUpgrade())
                {
                    var bd = b.GetBuildingData();
                    var cost = bd.GetBuildCost(b.GetUpgradeLevel() + 1);
                    ResourceData rd;
                    if (m_vIsAltResource == 0)
                        rd = bd.GetBuildResource(b.GetUpgradeLevel() + 1);
                    else
                        rd = bd.GetAltBuildResource(b.GetUpgradeLevel() + 1);
                    if (ca.HasEnoughResources(rd, cost))
                    {
                        if (level.HasFreeWorkers())
                        {
                            string name = b.GetData().GetName();
                            if (string.Equals(name, "Alliance Castle"))
                            {
                                ca.IncrementAllianceCastleLevel();
                                ca.SetAllianceCastleTotalCapacity(bd.GetUnitStorageCapacity(ca.GetAllianceCastleLevel()));
                            }
                            else if (string.Equals(name, "Town Hall"))
                                ca.IncrementTownHallLevel();
                            ca.SetResourceCount(rd, ca.GetResourceCount(rd) - cost);
                            b.StartUpgrading();
                        }
                    }
                }
            }
        }
        readonly List<int> m_vBuildingIdList;
        readonly byte m_vIsAltResource;
    }
}