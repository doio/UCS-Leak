using System;
using System.IO;
using System.Text;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;
using  UCS.Packets.Commands.Server;
using System.Threading.Tasks;

namespace UCS.Packets.Messages.Client
{
    // Packet 14316
    internal class EditClanSettingsMessage : Message
    {
        public EditClanSettingsMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        int m_vAllianceBadgeData;
        string m_vAllianceDescription;
        int m_vAllianceOrigin;
        int m_vAllianceType;
        int m_vRequiredScore;
        int m_vWarFrequency;
        byte m_vWarAndFriendlyStatus;

        internal override void Decode()
        {
            this.m_vAllianceDescription = this.Reader.ReadString();
            this.Reader.ReadInt32();
            this.m_vAllianceBadgeData = this.Reader.ReadInt32();
            this.m_vAllianceType = this.Reader.ReadInt32();
            this.m_vRequiredScore = this.Reader.ReadInt32();
            this.m_vWarFrequency = this.Reader.ReadInt32();
            this.m_vAllianceOrigin = this.Reader.ReadInt32();
            this.m_vWarAndFriendlyStatus = this.Reader.ReadByte();
        }

        internal override async void Process()
        {
            try
            {
                Alliance alliance = await ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceID);
                if (alliance != null)
                {
                    if (m_vAllianceDescription.Length < 259 || m_vAllianceDescription.Length < 0)
                    {
                        if (m_vAllianceBadgeData < 1 || m_vAllianceBadgeData < 10000000000)
                        {
                            if (m_vAllianceType < 0 || m_vAllianceType < 10)
                            {
                                if (m_vRequiredScore < 0 || m_vRequiredScore < 4201)
                                {
                                    if (m_vWarFrequency < 0 || m_vWarFrequency < 10)
                                    {
                                        if (m_vAllianceOrigin < 0 || m_vAllianceOrigin < 42000000)
                                        {
                                            if (m_vWarAndFriendlyStatus < 0 || m_vWarAndFriendlyStatus < 5)
                                            {
                                                alliance.SetAllianceDescription(m_vAllianceDescription);
                                                alliance.SetAllianceBadgeData(m_vAllianceBadgeData);
                                                alliance.SetAllianceType(m_vAllianceType);
                                                alliance.SetRequiredScore(m_vRequiredScore);
                                                alliance.SetWarFrequency(m_vWarFrequency);
                                                alliance.SetAllianceOrigin(m_vAllianceOrigin);
                                                alliance.SetWarAndFriendlytStatus(m_vWarAndFriendlyStatus);

                                                ClientAvatar avatar = this.Device.Player.Avatar;
                                                long allianceId = avatar.AllianceID;
                                                AllianceEventStreamEntry eventStreamEntry = new AllianceEventStreamEntry();
                                                eventStreamEntry.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                                                eventStreamEntry.SetSender(avatar);
                                                eventStreamEntry.SetEventType(10);
                                                eventStreamEntry.SetAvatarId(avatar.UserID);
                                                eventStreamEntry.SetUsername(avatar.Username);
                                                eventStreamEntry.SetSenderId(avatar.UserID);
                                                eventStreamEntry.SetSenderName(avatar.Username);
                                                alliance.AddChatMessage(eventStreamEntry);

                                                AllianceSettingChangedCommand edit = new AllianceSettingChangedCommand(this.Device);
                                                edit.SetAlliance(alliance);
                                                edit.SetPlayer(this.Device.Player);

                                                new AvailableServerCommandMessage(this.Device, edit.Handle()).Send();

                                                foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                                                {
                                                    Level user = await ResourcesManager.GetPlayer(op.GetAvatarId());
                                                    if (ResourcesManager.IsPlayerOnline(user))
                                                    {
                                                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(user.Client);
                                                        p.SetStreamEntry(eventStreamEntry);
                                                        p.Send();
                                                    }
                                                }

                                                DatabaseManager.Single().Save(alliance);
                                            }
                                            else
                                            {
                                                ResourcesManager.DisconnectClient(Device);
                                            }
                                        }
                                        else
                                        {
                                            ResourcesManager.DisconnectClient(Device);
                                        }
                                    }
                                    else
                                    {
                                        ResourcesManager.DisconnectClient(Device);
                                    }
                                }
                                else
                                {
                                    ResourcesManager.DisconnectClient(Device);
                                }
                            }
                            else
                            {
                                ResourcesManager.DisconnectClient(Device);
                            }
                        }
                        else
                        {
                            ResourcesManager.DisconnectClient(Device);
                        }
                    }
                    else
                    {
                        ResourcesManager.DisconnectClient(Device);
                    }
                }
            } catch (Exception) { }
        }
    }
}
