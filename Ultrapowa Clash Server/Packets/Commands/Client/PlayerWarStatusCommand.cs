using UCS.Helpers.Binary;

namespace UCS.Packets.Commands.Client
{
    internal class PlayerWarStatusCommand : Command
    {
        public PlayerWarStatusCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }
    }
}
