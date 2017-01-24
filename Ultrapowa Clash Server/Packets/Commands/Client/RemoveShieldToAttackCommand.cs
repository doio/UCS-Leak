using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    class RemoveShieldToAttackCommand : Command
    {
        public RemoveShieldToAttackCommand(PacketReader br)
        {
            //Todo
        }

        public int Tic { get; set; }

        public override void Execute(Level level)
        {
        }
    }
}
