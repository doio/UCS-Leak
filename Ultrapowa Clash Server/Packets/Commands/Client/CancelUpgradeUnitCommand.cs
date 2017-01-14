using System.IO;
using UCS.Helpers;

namespace UCS.Packets.Commands.Client
{
    // Packet 515
    internal class CancelUpgradeUnitCommand : Command
    {
        public CancelUpgradeUnitCommand(PacketReader br)
        {
            /*
            BuildingId = br.ReadUInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadUInt32WithEndian();
            */
        }

        public uint BuildingId { get; set; }
        public uint Unknown1 { get; set; }
    }
}