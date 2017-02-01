using System.IO;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10108
    internal class KeepAliveMessage : Message
    {
        public KeepAliveMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Process(Level level)
        {
            PacketManager.Send(new KeepAliveOkMessage(Client, this));
        }
    }
}