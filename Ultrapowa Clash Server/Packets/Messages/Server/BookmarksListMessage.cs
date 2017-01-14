using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.DataSlots;

namespace UCS.Packets.Messages.Server
{
    // Packet 24341
    internal class BookmarkListMessage : Message
    {
        public ClientAvatar player { get; set; }
        public int i;

        public BookmarkListMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24341);
            player = client.GetLevel().GetPlayerAvatar();
            i = 0;
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            List<byte> list = new List<byte>();
            List<BookmarkSlot> rem = new List<BookmarkSlot>();

            foreach(var p in player.BookmarkedClan)
            {
                Alliance a = ObjectManager.GetAlliance(p.Value);
                if (a != null)
                {
                    list.AddRange(ObjectManager.GetAlliance(p.Value).EncodeFullEntry());
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

            foreach(BookmarkSlot im in rem)
            {
                player.BookmarkedClan.RemoveAll(t => t == im);
            }
        }
    }
}
