using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 538
    internal class MyLeagueCommand : Command
    {
        public MyLeagueCommand(PacketReader br)
        {

        }

        public override void Execute(Level level)
        {
            //new LeaguePlayersMessage(level.GetClient()).Send();
        }
    }
}
