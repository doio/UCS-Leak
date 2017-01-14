using System;
using System.IO;
using System.Text;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 14201
    internal class FacebookLinkMessage : Message
    {
        public FacebookLinkMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                // TODO
            }
        }

        public override void Process(Level level)
        {
        }
    }
}