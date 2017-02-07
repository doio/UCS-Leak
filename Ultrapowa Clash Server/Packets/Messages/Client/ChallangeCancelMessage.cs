using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeCancelMessage : Message
    {
        public ChallangeCancelMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            Alliance a = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            StreamEntry s = a.GetChatMessages().Find(c => c.GetSenderId() == level.GetPlayerAvatar().GetId() && c.GetStreamEntryType() == 12);

            if (s != null)
            {
                a.GetChatMessages().RemoveAll(t => t == s);
                foreach (AllianceMemberEntry op in a.GetAllianceMembers())
                {
                    Level player = ResourcesManager.GetPlayer(op.GetAvatarId());
                    if (player.GetClient() != null)
                    {
                        PacketProcessor.Send(new AllianceStreamEntryRemovedMessage(Client, s.GetId()));
                    }                                                   
                }
            }
        }

    }
}
