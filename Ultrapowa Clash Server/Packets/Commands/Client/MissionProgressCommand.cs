using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 519
    internal class MissionProgressCommand : Command
    {
        public MissionProgressCommand(PacketReader br)
        {
            MissionID = br.ReadUInt32();
            Tick = br.ReadUInt32();
        }

        public uint MissionID { get; set; }

        public uint Tick { get; set; }

        public override void Execute(Level level)
        {
        }
    }
}