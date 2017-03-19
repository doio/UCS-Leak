using System;
using System.Collections.Generic;
using UCS.Core;
using UCS.Helpers.List;
using UCS.Logic;
using UCS.Logic.DataSlots;

namespace UCS.Packets.Messages.Server
{
    // Packet 24341
    internal class BookmarkListMessage : Message
    {
        public ClientAvatar Player;
        public int I;

        public BookmarkListMessage(Device client) : base(client)
        {
            this.Identifier = 24341;
            this.Player = client.Player.Avatar;
            I = 0;
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
                        list.AddRange(a.EncodeFullEntry());
                        I++;
                    }
                    else
                    {
                        rem.Add(p);
                        if (I > 0)
                            I--;
                    }
                }

                this.Data.AddInt(I);
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
