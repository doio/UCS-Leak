using System.IO;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 14322
    internal class AllianceInviteMessage : Message
    {
        public AllianceInviteMessage(Device device, Reader reader) : base(device, reader)
        {
        }

    }
}