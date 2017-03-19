using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Helpers.List;
using UCS.Logic;
using UCS.Logic.DataSlots;

namespace UCS.Packets.Messages.Server
{
    // Packet 24340
    internal class BookmarkMessage : Message
    {
        public ClientAvatar Player;
        public int i;

        public BookmarkMessage(Device client) : base(client)
        {
            this.Identifier = 24340;
            Player = client.Player.Avatar;
        }

        internal override async void Encode()
        {
            try
            {
                List<byte> list = new List<byte>();
                List<long> rem = new List<long>();
                foreach (var p in Player.Bookmark)
                {
                    Alliance a = await ObjectManager.GetAlliance(p);
                    if (a != null)
                    {
                        list.AddLong(p);
                        i++;
                    }
                    else
                    {
                        rem.Add(p);
                        if (i > 0)
                            i--;
                    }
                }
                this.Data.AddInt(i);
                this.Data.AddRange(list);
                foreach (var im in rem)
                {
                    Player.Bookmark.RemoveAll(t => t == im);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}