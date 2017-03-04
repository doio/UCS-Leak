using System.IO;
using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet ?
    internal class FetchWarBaseMessage : Message
    {
        public FetchWarBaseMessage(Device device, Reader reader) : base(device, reader)
        {
        }
    }
}