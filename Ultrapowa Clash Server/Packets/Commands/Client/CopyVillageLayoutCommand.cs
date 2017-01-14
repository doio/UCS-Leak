using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 568
    internal class CopyVillageLayoutCommand : Command
    {
        public CopyVillageLayoutCommand(PacketReader br)
        {
            Tick = br.ReadInt32WithEndian();
            CopiedLayoutID = br.ReadInt32WithEndian();
            PasteLayoutID = br.ReadInt32WithEndian();
        }

        public int PasteLayoutID { get; set; }

        public int CopiedLayoutID { get; set; }

        public int Tick { get; set; }

        public override void Execute(Level level)
        {
        }
    }
}