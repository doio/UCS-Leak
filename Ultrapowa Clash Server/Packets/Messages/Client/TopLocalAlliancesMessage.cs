using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14402
    internal class TopLocalAlliancesMessage : Message
    {
        public TopLocalAlliancesMessage(Device device, Reader reader) : base(device, reader)
        {
        }


        internal override void Process()
        {
            new LocalAlliancesMessage(Device).Send();
        }
    }
}
