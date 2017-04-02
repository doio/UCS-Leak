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
                long allianceID = player.AllianceId;
                Alliance alliance = ObjectManager.GetAlliance(allianceID);

                ChallengeStreamEntry cm = new ChallengeStreamEntry() { m_vMessage = Message };
                cm.SetSender(player);
                cm.SetId(alliance.m_vChatMessages.Count + 1);
                alliance.AddChatMessage((ChallengeStreamEntry)cm);

                StreamEntry s = alliance.m_vChatMessages.Find(c => c.GetStreamEntryType() == 12);
                if (s != null)
                {
                    alliance.m_vChatMessages.RemoveAll(t => t == s);

                    foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                    {
                        Level alliancemembers = await ResourcesManager.GetPlayer(op.AvatarId);
                        if (alliancemembers.Client != null)
                        {
                            new AllianceStreamEntryRemovedMessage(alliancemembers.Client, s.m_vId).Send();
                            AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(alliancemembers.Client);
                            p.SetStreamEntry(cm);
                            p.Send();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
