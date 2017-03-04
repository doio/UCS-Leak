using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Commands.Client;
using UCS.Packets.Messages.Server;
using UCS.Packets.Commands.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14301
    internal class CreateAllianceMessage : Message
    {
        public CreateAllianceMessage(Device device, Reader reader) : base(device, reader)
        {

        }

        int m_vAllianceBadgeData;
        string m_vAllianceDescription;
        string m_vAllianceName;
        int m_vAllianceOrigin;
        int m_vAllianceType;
        int m_vRequiredScore;
        int m_vWarFrequency;
        byte m_vWarAndFriendlyStatus;

        internal override void Decode()
        {
                this.m_vAllianceName = this.Reader.ReadString();
                this.m_vAllianceDescription = this.Reader.ReadString();
                this.m_vAllianceBadgeData = this.Reader.ReadInt32();
                this.m_vAllianceType = this.Reader.ReadInt32();
                this.m_vRequiredScore = this.Reader.ReadInt32();
                this.m_vWarFrequency = this.Reader.ReadInt32();
                this.m_vAllianceOrigin = this.Reader.ReadInt32();
                this.m_vWarAndFriendlyStatus = this.Reader.ReadByte();
            
        }

        internal override void Process()
        {
            if (m_vAllianceName == null)
                m_vAllianceName = "Clan";

            if (m_vAllianceName.Length < 16 || m_vAllianceName.Length < 1)
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
                                            Alliance alliance = ObjectManager.CreateAlliance(0);
                                            alliance.SetAllianceName(m_vAllianceName);
                                            alliance.SetAllianceDescription(m_vAllianceDescription);
                                            alliance.SetAllianceType(m_vAllianceType);
                                            alliance.SetRequiredScore(m_vRequiredScore);
                                            alliance.SetAllianceBadgeData(m_vAllianceBadgeData);
                                            alliance.SetAllianceOrigin(m_vAllianceOrigin);
                                            alliance.SetWarFrequency(m_vWarFrequency);
                                            alliance.SetWarAndFriendlytStatus(m_vWarAndFriendlyStatus);
                                            this.Device.Player.Avatar.SetAllianceId(alliance.GetAllianceId());

                                            AllianceMemberEntry member = new AllianceMemberEntry(this.Device.Player.Avatar.GetId());
                                            member.SetRole(2);
                                            alliance.AddAllianceMember(member);

                                            JoinedAllianceCommand b = new JoinedAllianceCommand(this.Device);
                                            b.SetAlliance(alliance);

                                            AllianceRoleUpdateCommand d = new AllianceRoleUpdateCommand(this.Device);
                                            d.SetAlliance(alliance);
                                            d.SetRole(2);
                                            d.Tick(this.Device.Player);

                                            new AvailableServerCommandMessage(this.Device, b.Handle()).Send();

                                            new AvailableServerCommandMessage(this.Device, d.Handle()).Send();
  
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
            else
            {
                ResourcesManager.DisconnectClient(Device);
            }
        }
    }
}
