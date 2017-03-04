using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14404
    internal class TopLocalPlayersMessage : Message
    {
        public TopLocalPlayersMessage(Device device, Reader reader) : base(device, reader)
        {
        }


        internal override void Process()
        {
            new LocalPlayersMessage(Device).Send();
        }
    }
}
