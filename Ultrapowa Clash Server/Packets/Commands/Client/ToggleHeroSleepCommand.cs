using System.IO;
using UCS.Helpers;

namespace UCS.Packets.Commands.Client
{
    // Packet 529
    internal class ToggleHeroSleepCommand : Command
    {
        public ToggleHeroSleepCommand(PacketReader br)
        {
            BuildingId = br.ReadUInt32WithEndian(); 
            FlagSleep = br.ReadByte();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public uint BuildingId { get; set; }
        public byte FlagSleep { get; set; }
        public uint Unknown1 { get; set; }
    }
}