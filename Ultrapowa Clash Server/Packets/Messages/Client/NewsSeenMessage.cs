using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 10905
    internal class NewsSeenMessage : Message
    {
        public NewsSeenMessage(Packets.Client client, PacketReader br) : base(client, br)
        {

        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {  
            }
        }

        public override void Process(Level level)
        {

        }
    }
}
