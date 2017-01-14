using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    internal class AddClashFriendMessage : Message
    {
        public AddClashFriendMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                 FriendID = br.ReadInt64();  
            }
        }

        public long FriendID { get; set; }

        public override void Process(Level level)
        {
        }
    }
}
