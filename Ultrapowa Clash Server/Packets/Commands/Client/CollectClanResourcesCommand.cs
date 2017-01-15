using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    class CollectClanResourcesCommand : Command
    {
        public CollectClanResourcesCommand(PacketReader br)
        {
            Tick = br.ReadInt32WithEndian();
        }

        public int Tick { get; set; }

        public override void Execute(Level level)
        {
            // Database change is needed for the Player
        }
    }
}
