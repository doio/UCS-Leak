using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 603
    internal class EndOfBattleCommand : Command
    {
        public EndOfBattleCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }
    }
}