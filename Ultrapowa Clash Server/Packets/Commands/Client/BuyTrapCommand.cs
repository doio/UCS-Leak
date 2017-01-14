using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 510
    internal class BuyTrapCommand : Command
    {
        public BuyTrapCommand(PacketReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            TrapId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            var td = (TrapData)CSVManager.DataTables.GetDataById(TrapId);
            var t = new Trap(td, level);

            if (ca.HasEnoughResources(td.GetBuildResource(0), td.GetBuildCost(0)))
            {
                if (level.HasFreeWorkers())
                {
                    var rd = td.GetBuildResource(0);
                    ca.CommodityCountChangeHelper(0, rd, -td.GetBuildCost(0));

                    t.StartConstructing(X, Y);
                    level.GameObjectManager.AddGameObject(t);
                }
            }
        }

        public int TrapId { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}