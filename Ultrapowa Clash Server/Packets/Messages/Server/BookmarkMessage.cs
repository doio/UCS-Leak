using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.DataSlots;

namespace UCS.Packets.Messages.Server
{
    // Packet 24340
    internal class BookmarkMessage : Message
    {
        public ClientAvatar player { get; set; }
        public int i;

        public BookmarkMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24340);
            player = client.GetLevel().GetPlayerAvatar();
        }

        public override async void Encode()
        {
            try
            {
                List<byte> data = new List<byte>();
                List<byte> list = new List<byte>();
                List<BookmarkSlot> rem = new List<BookmarkSlot>();
                foreach (var p in player.BookmarkedClan)
                {
                    Alliance a = await ObjectManager.GetAlliance(p.Value);
                    if (a != null)
                    {
                        list.AddInt64(p.Value);
                        i++;
                    }
                    else
                    {
                        rem.Add(p);
                        if (i > 0)
                            i--;
                    }
                }
                data.AddInt32(i);
                data.AddRange(list);
                Encrypt(data.ToArray());
                foreach (BookmarkSlot im in rem)
                {
                    player.BookmarkedClan.RemoveAll(t => t == im);
                }
            } catch (Exception) { }
        }
    }
}
