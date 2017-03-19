using System;
using System.Collections.Generic;
using System.IO;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.DataSlots;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14344
    internal class RemoveFromBookmarkMessage : Message
    {
        public RemoveFromBookmarkMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        private long id;

        internal override void Decode()
        {
            this.id = this.Reader.ReadInt64();
        }

        internal override void Process()
        {
            var al = this.Device.Player.Avatar.Bookmark.Find(a => a == id);
            if (al > 0)
            {
                this.Device.Player.Avatar.Bookmark.Remove(al);
            }
            new BookmarkRemoveAllianceMessage(Device).Send();
        } 
    }
}