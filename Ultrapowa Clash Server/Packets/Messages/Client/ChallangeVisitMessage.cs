using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeVisitMessage : Message
    {
        public ChallangeVisitMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public long AvatarID { get; set; }

        public override void Decode()
        {
            using (PacketReader r = new PacketReader(new MemoryStream(GetData())))
            {
                AvatarID = r.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
        {
            new OwnHomeDataMessage(Client, level).Send();		
            //var defender = ResourcesManager.GetPlayer(AvatarID); // TODO: FIX BUGS		
            //PacketManager.ProcessOutgoingPacket(new VisitedHomeDataMessage(Client, defender, level)); 
        }
    }
}
