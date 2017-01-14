using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 554
    internal class RotateDefenseCommand : Command
    {
        public RotateDefenseCommand(PacketReader br)
        {
            BuildingID = br.ReadInt32();
            //Console.WriteLine(br.ReadInt32());
            //Console.WriteLine(br.ReadInt64()); // Unknown
            //Console.WriteLine(br.ReadInt32()); // Tick
        }

        public int BuildingID { get; set; }

        public override void Execute(Level level)
        {
            //Console.WriteLine(BuildingID);
        }
    }
}
