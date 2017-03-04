using System.IO;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10113
    internal class GetDeviceTokenMessage : Message
    {
        public GetDeviceTokenMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Process()
        {
            new SetDeviceTokenMessage(Device).Send();
        }
    }
}