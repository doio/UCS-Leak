using System.IO;
using UCS.Helpers;

namespace UCS.Packets.Commands.Client
{
    // Packet ?
    internal class DonateAllianceUnitCommand : Command
    {
        public DonateAllianceUnitCommand(PacketReader br)
        {
            /*Unknown1 = br.ReadUInt32WithEndian();
            PlayerId = br.ReadUInt32WithEndian();
            UnitType = br.ReadUInt32WithEndian();
            Unknown2 = br.ReadUInt32WithEndian();
            Unknown3 = br.ReadUInt32WithEndian();*/
        }

        public uint PlayerId { get; set; }
        public uint UnitType { get; set; }
        public uint Unknown1 { get; set; } 
        public uint Unknown2 { get; set; } 
        public uint Unknown3 { get; set; }
    }
}
