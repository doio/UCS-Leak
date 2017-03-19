using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands
{
    internal class ChallangeCommand : Command
    {
        public string Message;

        public ChallangeCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.Message = this.Reader.ReadString();
        }

        internal override async void Process()
        {
            try
            {
                ClientAvatar player = this.Device.Player.Avatar;
                long allianceID = player.AllianceID;
                Alliance alliance = await ObjectManager.GetAlliance(allianceID);

                ChallengeStreamEntry cm = new ChallengeStreamEntry();
                cm.SetMessage(Message);
                cm.SetSenderId(player.UserID);
                cm.SetSenderName(player.Username);
                cm.SetSenderLevel(player.Level);
                cm.SetSenderRole(await player.GetAllianceRole());
                cm.SetId(alliance.GetChatMessages().Count + 1);
                cm.SetSenderLeagueId(player.League);

                alliance.AddChatMessage((ChallengeStreamEntry)cm);

                StreamEntry s = alliance.GetChatMessages().Find(c => c.GetStreamEntryType() == 12);
                if (s != null)
                {
                    alliance.GetChatMessages().RemoveAll(t => t == s);

                    foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                    {
                        Level alliancemembers = await ResourcesManager.GetPlayer(op.GetAvatarId());
                        if (alliancemembers.Client != null)
                        {
                            new AllianceStreamEntryRemovedMessage(alliancemembers.Client, s.GetId()).Send();
                        }
                    }
                }

                foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                {
                    Level alliancemembers = await ResourcesManager.GetPlayer(op.GetAvatarId());
                    if (alliancemembers.Client != null)
                    {
                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(alliancemembers.Client);
                        p.SetStreamEntry(cm);
                        p.Send();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
