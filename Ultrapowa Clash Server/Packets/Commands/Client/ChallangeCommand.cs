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

namespace UCS.Packets.Commands
{
    internal class ChallangeCommand : Command
    {
        public string Message { get; set; }

        public ChallangeCommand(PacketReader br)
        {
            Message = br.ReadString();
        }

        public override void Execute(Level level)
        {
            ClientAvatar player = level.GetPlayerAvatar();
            long allianceID = player.GetAllianceId();
            Alliance alliance = ObjectManager.GetAlliance(allianceID);

            ChallangeStreamEntry cm = new ChallangeStreamEntry();
            cm.SetMessage(Message);
            cm.SetSenderId(player.GetId());
            cm.SetSenderName(player.GetAvatarName());
            cm.SetSenderLevel(player.GetAvatarLevel());
            cm.SetSenderRole(player.GetAllianceRole());
            cm.SetId(alliance.GetChatMessages().Count + 1);
            cm.SetSenderLeagueId(player.GetLeagueId());

            StreamEntry s = alliance.GetChatMessages().Find(c => c.GetStreamEntryType() == 12);
            if (s != null)
            {
                alliance.GetChatMessages().RemoveAll(t => t == s);

                foreach(AllianceMemberEntry op in alliance.GetAllianceMembers())
                {
                    Level alliancemembers = ResourcesManager.GetPlayer(op.GetAvatarId());
                    if (alliancemembers.GetClient() != null)
                    {
                        new AllianceStreamEntryRemovedMessage(alliancemembers.GetClient(), s.GetId()).Send();
                    }
                }
            }

            alliance.AddChatMessage(cm);

            foreach(AllianceMemberEntry op in alliance.GetAllianceMembers())
            {
                Level alliancemembers = ResourcesManager.GetPlayer(op.GetAvatarId());
                if (alliancemembers.GetClient() != null)
                {
                    AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(alliancemembers.GetClient());
                    p.SetStreamEntry(cm);
                    p.Send();
                }
            }
        }
    }
}
