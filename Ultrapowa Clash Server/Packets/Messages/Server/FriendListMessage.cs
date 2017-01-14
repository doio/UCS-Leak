using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 20105
    internal class FriendListMessage : Message
    {
        public FriendListMessage(Packets.Client client) : base(client)
        {
            SetMessageType(20105);
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddInt32(0);
            pack.AddInt32(0);
            pack.AddDataSlots(new List<DataSlot>());
            Encrypt(pack.ToArray());
        }
    }
}