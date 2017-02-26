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
    class AccountSwitchMessage : Message
    {
        public AccountSwitchMessage(Packets.Client _Client, PacketReader _PacketReader) : base(_Client, _PacketReader)
        {
        }

        public override void Decode()
        {
            using (PacketReader _Reader = new PacketReader(new MemoryStream(GetData())))
            {
            }
        }

        public override async void Process(Level level)
        {
        }
    }
}
