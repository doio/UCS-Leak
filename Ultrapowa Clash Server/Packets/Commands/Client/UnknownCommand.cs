using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 3072
    internal class UnknownCommand : Command
    {
        public UnknownCommand(PacketReader br)
        {
            //Unknown1 = br.ReadInt32();
            //Tick = br.ReadInt32();
            //Packet = br.ReadAllBytes();
        }

        public override void Execute(Level level)
        {
            //Console.WriteLine("[CMD][0]     " + Unknown1);
            //Console.WriteLine("[CMD][0]     " + Tick);
            //Console.WriteLine("[CMD][0][FULL] " + Encoding.ASCII.GetString(Packet));
        }

        public static byte[] Packet { get; set; }
        public static int Tick { get; set; }
        public static int Unknown1 { get; set; }
    }
}