using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 512
    internal class BuyDecoCommand : Command
    {
        public BuyDecoCommand(PacketReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            DecoId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            ClientAvatar ca = level.GetPlayerAvatar();

            DecoData dd = (DecoData)CSVManager.DataTables.GetDataById(DecoId);

            if (ca.HasEnoughResources(dd.GetBuildResource(), dd.GetBuildCost()))
            {
                ResourceData rd = dd.GetBuildResource();
                ca.CommodityCountChangeHelper(0, rd, -dd.GetBuildCost());

                Deco d = new Deco(dd, level);
                d.SetPositionXY(X, Y, level.GetPlayerAvatar().GetActiveLayout());
                level.GameObjectManager.AddGameObject(d);
            }
        }

        public int DecoId { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}