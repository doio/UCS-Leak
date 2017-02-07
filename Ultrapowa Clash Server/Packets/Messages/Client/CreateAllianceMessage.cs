using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Commands.Client;
using UCS.Packets.Messages.Server;
using UCS.Packets.Commands.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14301
    internal class CreateAllianceMessage : Message
    {
        public CreateAllianceMessage(Packets.Client client, PacketReader br) : base(client, br)
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

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                m_vAllianceName = br.ReadString();
                m_vAllianceDescription = br.ReadString();
                m_vAllianceBadgeData = br.ReadInt32WithEndian();
                m_vAllianceType = br.ReadInt32WithEndian();
                m_vRequiredScore = br.ReadInt32WithEndian();
                m_vWarFrequency = br.ReadInt32WithEndian();
                m_vAllianceOrigin = br.ReadInt32WithEndian();
                m_vWarAndFriendlyStatus = br.ReadByte();
            }
        }

        public override void Process(Level level)
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
                                            level.GetPlayerAvatar().SetAllianceId(alliance.GetAllianceId());

                                            AllianceMemberEntry member = new AllianceMemberEntry(level.GetPlayerAvatar().GetId());
                                            member.SetRole(2);
                                            alliance.AddAllianceMember(member);

                                            JoinedAllianceCommand b = new JoinedAllianceCommand();
                                            b.SetAlliance(alliance);

                                            AllianceRoleUpdateCommand d = new AllianceRoleUpdateCommand();
                                            d.SetAlliance(alliance);
                                            d.SetRole(2);
                                            d.Tick(level);

                                            AvailableServerCommandMessage a = new AvailableServerCommandMessage(Client);
                                            a.SetCommandId(1);
                                            a.SetCommand(b);

                                            AvailableServerCommandMessage c = new AvailableServerCommandMessage(Client);
                                            c.SetCommandId(8);
                                            c.SetCommand(d);

                                            PacketProcessor.Send(a);
                                            PacketProcessor.Send(c);
                                        }
                                        else
                                        {
                                            ResourcesManager.DisconnectClient(Client);
                                        }
                                    }
                                    else
                                    {
                                        ResourcesManager.DisconnectClient(Client);
                                    }
                                }
                                else
                                {
                                    ResourcesManager.DisconnectClient(Client);
                                }
                            }
                            else
                            {
                                ResourcesManager.DisconnectClient(Client);
                            }
                        }
                        else
                        {
                            ResourcesManager.DisconnectClient(Client);
                        }
                    }
                    else
                    {
                        ResourcesManager.DisconnectClient(Client);
                    }
                }
                else
                {
                    ResourcesManager.DisconnectClient(Client);
                }
            }
            else
            {
                ResourcesManager.DisconnectClient(Client);
            }
        }
    }
}
