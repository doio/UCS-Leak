using System.IO;
using UCS.Helpers;

namespace UCS.Packets.Commands.Client
{
    // Packet 572
    internal class ToggleHeroAttackModeCommand : Command
    {
        public ToggleHeroAttackModeCommand(PacketReader br)
        {
            /*
            BuildingId = br.ReadUInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadByte();
            Unknown2 = br.ReadUInt32WithEndian();
            Unknown3 = br.ReadUInt32WithEndian();
            */
        }

        public uint BuildingId { get; set; }
        public byte Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
    }
}