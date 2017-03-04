using System.Collections.Generic;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 20105
    internal class FriendListMessage : Message
    {
        public FriendListMessage(Device client) : base(client)
        {
            this.Identifier = 20105;
        }

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Data.AddInt(0);
            this.Data.AddDataSlots(new List<DataSlot>());
        }
    }
}