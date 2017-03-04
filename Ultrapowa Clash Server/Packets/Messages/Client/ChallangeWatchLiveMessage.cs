using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeWatchLiveMessage : Message
    {
        public ChallangeWatchLiveMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Process()
        {
            new OwnHomeDataMessage(Device, this.Device.Player).Send();
        }
    }
}
