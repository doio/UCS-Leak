using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    class SpeedUpRequestUnitsCommand : Command
    {
        public SpeedUpRequestUnitsCommand(PacketReader br)
        {
            Tick = br.ReadInt32();
        }

        public int Tick { get; set; }

        public override void Execute(Level level)
        {
        }
    }
}
