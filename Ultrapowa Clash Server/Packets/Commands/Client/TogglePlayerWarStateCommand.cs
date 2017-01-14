using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 570
    internal class TogglePlayerWarStateCommand : Command
    {
        public TogglePlayerWarStateCommand(PacketReader br)
        {
            br.ReadInt32();
            br.ReadInt32();
        }

        public override void Execute(Level level)
        {
            /*
            var p = new PlayerWarStatusMessage();
            p.SetStatus(0);

            var a = new AvailableServerCommandMessage(level.GetClient());
            a.SetCommandId(14);
            a.SetCommand(p);
            PacketManager.ProcessOutgoingPacket(a);*/
            //TODO
        }
    }
}