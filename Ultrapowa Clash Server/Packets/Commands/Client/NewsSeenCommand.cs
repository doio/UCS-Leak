using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 539
    internal class NewsSeenCommand : Command
    {
        public byte[] packet;

        public NewsSeenCommand(PacketReader br)
        {
            //packet = br.ReadAllBytes();
            //Unknown1 = br.ReadUInt32WithEndian();
            //Unknown2 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
        }

        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
    }
}