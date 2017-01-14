using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 501
    internal class MoveBuildingCommand : Command
    {
        public MoveBuildingCommand(PacketReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            BuildingId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            GameObject go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            go.SetPositionXY(X, Y, level.GetPlayerAvatar().GetActiveLayout());
        }

        public int BuildingId { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; } 
        public int Y { get; set; }
    }
}