using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;
using UCS.Packets.Commands.Server;
using System.Threading.Tasks;
using UCS.Helpers.Binary;

namespace UCS.Packets.Commands.Client
{
    // Packet 543
    internal class KickAllianceMemberCommand : Command
    {
        public KickAllianceMemberCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.m_vAvatarId = this.Reader.ReadInt64();
            this.Reader.ReadByte();
            this.m_vMessage = this.Reader.ReadString();
            this.Reader.ReadInt32();
        }

        internal override async void Process()
        {
            try
            {
                var targetAccount = await ResourcesManager.GetPlayer(m_vAvatarId);
                if (targetAccount != null)
                {
                    var targetAvatar = targetAccount.Avatar;
                    var targetAllianceId = targetAvatar.AllianceID;
                    var requesterAvatar = this.Device.Player.Avatar;
                    var requesterAllianceId = requesterAvatar.AllianceID;
                    if (requesterAllianceId > 0 && targetAllianceId == requesterAllianceId)
                    {
                        var alliance = await ObjectManager.GetAlliance(requesterAllianceId);
                        var requesterMember = alliance.GetAllianceMember(requesterAvatar.UserID);
                        var targetMember = alliance.GetAllianceMember(m_vAvatarId);
                        if (targetMember.HasLowerRoleThan(requesterMember.Role))
                        {
                            targetAvatar.AllianceID = 0;
                            alliance.RemoveMember(m_vAvatarId);
                            if (ResourcesManager.IsPlayerOnline(targetAccount))
                            {
                                var leaveAllianceCommand = new LeavedAllianceCommand(this.Device);
                                leaveAllianceCommand.SetAlliance(alliance);
                                leaveAllianceCommand.SetReason(2); //Kick
                                new AvailableServerCommandMessage(targetAccount.Client, leaveAllianceCommand.Handle()).Send();

                                var kickOutStreamEntry = new AllianceKickOutStreamEntry();
                                kickOutStreamEntry.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                                kickOutStreamEntry.SetAvatar(requesterAvatar);
                                kickOutStreamEntry.SetIsNew(0);
                                kickOutStreamEntry.SetAllianceId(alliance.AllianceID);
                                kickOutStreamEntry.SetAllianceBadgeData(alliance.GetAllianceBadgeData());
                                kickOutStreamEntry.SetAllianceName(alliance.GetAllianceName());
                                kickOutStreamEntry.SetMessage(m_vMessage);

                                var p = new AvatarStreamEntryMessage(targetAccount.Client);
                                p.SetAvatarStreamEntry(kickOutStreamEntry);
                                p.Send();
                            }

                            var eventStreamEntry = new AllianceEventStreamEntry();
                            eventStreamEntry.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                            eventStreamEntry.SetSender(targetAvatar);
                            eventStreamEntry.SetEventType(1);
                            eventStreamEntry.SetAvatarId(requesterAvatar.UserID);
                            eventStreamEntry.SetUsername(requesterAvatar.Username);
                            alliance.AddChatMessage(eventStreamEntry);

                            foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                            {
                                Level alliancemembers = await ResourcesManager.GetPlayer(op.AvatarID);
                                if (alliancemembers.Client != null)
                                {
                                    AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(alliancemembers.Client);
                                    p.SetStreamEntry(eventStreamEntry);
                                    p.Send();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal long m_vAvatarId;
        internal string m_vMessage;
    }
}
