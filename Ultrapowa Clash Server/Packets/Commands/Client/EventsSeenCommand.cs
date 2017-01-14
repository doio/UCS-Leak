using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    class EventsSeenCommand : Command
    {
        public EventsSeenCommand(PacketReader br)
        {
            UnknownID = br.ReadInt32WithEndian();
            Tick = br.ReadInt32WithEndian();
        }

        public int Tick { get; set; }

        public int UnknownID { get; set; }

        public override void Execute(Level level)
        {
        }
    }
}
