using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeWatchLiveMessage : Message
    {
        public ChallangeWatchLiveMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            PacketProcessor.Send(new OwnHomeDataMessage(Client, level));
        }
    }
}
