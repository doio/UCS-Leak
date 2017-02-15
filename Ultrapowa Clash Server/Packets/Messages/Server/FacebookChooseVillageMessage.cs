using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    class FacebookChooseVillageMessage : Message
    {
        public FacebookChooseVillageMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24262);
        }

        public override async void Encode()
        {
            List<byte> _data = new List<byte>();
            _data.AddInt32(2);
            Encrypt(_data.ToArray());
        }
    }
}
