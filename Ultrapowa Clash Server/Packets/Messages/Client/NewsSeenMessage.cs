using System;
using System.IO;
using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 10905
    internal class NewsSeenMessage : Message
    {
        public NewsSeenMessage(Device device, Reader reader) : base(device, reader)
        {

        }
    }
}
