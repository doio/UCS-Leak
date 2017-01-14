using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 553
    internal class ClientServerTickCommand : Command
    {
        public static int Tick { get; set; }
        public static int Unknown1 { get; set; }

        public ClientServerTickCommand(PacketReader br)
        {
            Unknown1 = br.ReadInt32();
            Tick = br.ReadInt32();
        }

        public override void Execute(Level level)
        {
        }
    }
}