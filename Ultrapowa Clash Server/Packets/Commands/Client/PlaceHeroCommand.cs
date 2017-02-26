using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 605
    internal class PlaceHeroCommand : Command
    {
        public PlaceHeroCommand(PacketReader br)
        {
            X      = br.ReadInt32();
            Y      = br.ReadInt32();
            HeroID = br.ReadInt32();
            Tick   = br.ReadInt32();
        }

        public override void Execute(Level level)
        {
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Tick { get; set; }

        public int HeroID { get; set; }
    }
}