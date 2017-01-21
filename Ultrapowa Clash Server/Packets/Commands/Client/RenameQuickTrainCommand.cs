using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    class RenameQuickTrainCommand : Command
    {
        public RenameQuickTrainCommand(PacketReader br)
        {
            SlotID = br.ReadInt32();
            SlotName = br.ReadString();
            Tick = br.ReadInt32();
        }

        public int SlotID { get; set; }

        public string SlotName { get; set; }

        public int Tick { get; set; }

        public override void Execute(Level level)
        {
            /*ClientAvatar pl = level.GetPlayerAvatar();

            if (SlotID == 1)
            {
            }
            else if (SlotID == 2)
            {
            }
            else if (SlotID == 3)
            {
            } */
        }
    }
}
