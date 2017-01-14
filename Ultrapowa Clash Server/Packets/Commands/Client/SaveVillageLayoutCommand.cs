using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 552
    internal class SaveVillageLayoutCommand : Command
    {
        public SaveVillageLayoutCommand(PacketReader br)
        {
            br.Read();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
        }

        public override void Execute(Level level)
        {
            DatabaseManager.Single().Save(level);
        }
    }
}
