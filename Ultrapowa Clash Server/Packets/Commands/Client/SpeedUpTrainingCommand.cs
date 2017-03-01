using System;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 513
    internal class SpeedUpTrainingCommand : Command
    {
        readonly int m_vBuildingId;

        public SpeedUpTrainingCommand(PacketReader br)
        {
            br.ReadInt32();
            br.ReadInt32(); // Troop Count?
            br.ReadUInt32();
        }

        public override void Execute(Level level)
        {
        }
    }
}