using UCS.Helpers.Binary;

namespace UCS.Packets.Commands.Client
{
    // Packet 544
    internal class GetVillageLayoutsCommand : Command
    {
        public GetVillageLayoutsCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

    }
}