using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    class AccountSwitchMessage : Message
    {
        public AccountSwitchMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
        }

        internal override async void Process()
        {
        }
    }
}
