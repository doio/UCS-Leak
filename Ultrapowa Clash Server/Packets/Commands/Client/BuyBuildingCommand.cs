using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 500
    internal class BuyBuildingCommand : Command
    {
        public BuyBuildingCommand(PacketReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            BuildingId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var bd = (BuildingData)CSVManager.DataTables.GetDataById(BuildingId);
            var b = new Building(bd, level);

            if (ca.HasEnoughResources(bd.GetBuildResource(0), bd.GetBuildCost(0)))
            {
                if (bd.IsWorkerBuilding() || level.HasFreeWorkers())
                {
                    var rd = bd.GetBuildResource(0);
                    ca.CommodityCountChangeHelper(0, rd, -bd.GetBuildCost(0));

                    b.StartConstructing(X, Y);
                    level.GameObjectManager.AddGameObject(b);
                }
            }
        }

        public int BuildingId { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}