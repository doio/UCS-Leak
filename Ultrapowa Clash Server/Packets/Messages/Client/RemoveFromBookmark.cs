using System;
using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.DataSlots;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14344
    internal class RemoveFromBookmarkMessage : Message
    {
        public RemoveFromBookmarkMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        private long id;

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                id = br.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
        {
            BookmarkSlot al = level.GetPlayerAvatar().BookmarkedClan.Find(a => a.Value == id);
            if (al != null)
            {
                level.GetPlayerAvatar().BookmarkedClan.Remove(al);
            }
            new BookmarkRemoveAllianceMessage(Client).Send();
        } 
    }
}