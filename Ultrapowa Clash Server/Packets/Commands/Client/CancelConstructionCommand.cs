using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 505
    internal class CancelConstructionCommand : Command
    {
        public CancelConstructionCommand(PacketReader br)
        {
            BuildingId = br.ReadInt32WithEndian(); 
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            if (go != null)
            {
                if (go.ClassId == 0 || go.ClassId == 4)
                {
                    var constructionItem = (ConstructionItem) go;
                    if (constructionItem.IsConstructing())
                    {
                        var ca = level.GetPlayerAvatar();
                        string name = level.GameObjectManager.GetGameObjectByID(BuildingId).GetData().GetName();
                        Logger.Write("Canceling Building Upgrade: " + name + " (" + BuildingId + ')');
                        if (string.Equals(name, "Alliance Castle"))
                        {
                            ca.DeIncrementAllianceCastleLevel();
                            Building a = (Building)go;
                            BuildingData al = a.GetBuildingData();
                            ca.SetAllianceCastleTotalCapacity(al.GetUnitStorageCapacity(ca.GetAllianceCastleLevel() - 1));
                        }
                        else if (string.Equals(name, "Town Hall"))
                            ca.DeIncrementTownHallLevel();

                        constructionItem.CancelConstruction();
                    }
                }
                else if (go.ClassId == 3)
                {
                }
            }
        }

        public int BuildingId { get; set; }
        public uint Unknown1 { get; set; }
    }
}
