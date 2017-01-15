using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 567
    internal class SetActiveVillageLayoutCommand : Command
    {
        private int Layout;
        public SetActiveVillageLayoutCommand(PacketReader br)
        {
            Layout = br.ReadInt32();
            //Console.WriteLine(br.ReadInt32());
            //Console.WriteLine(br.ReadInt32());
        }
        public override void Execute(Level level)
        {
            //level.GetPlayerAvatar().SetActiveLayout(Layout);
        }
    }
}
