using System.IO;
using UCS.Helpers;

namespace UCS.Packets.Commands.Client
{
    // Packet 532
    internal class NewShopItemsSeenCommand : Command
    {
        public NewShopItemsSeenCommand(PacketReader br)
        {
        }

        public uint NewShopItemNumber { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
    }
}