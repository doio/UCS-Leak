using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Commands.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14306
    internal class PromoteAllianceMemberMessage : Message
    {
        public PromoteAllianceMemberMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public long m_vId;
        public int m_vRole;

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                m_vId = br.ReadInt64WithEndian();
                m_vRole = br.ReadInt32WithEndian();
            }
        }

        public override void Process(Level level)
        {
            Level target = ResourcesManager.GetPlayer(m_vId);
            ClientAvatar player = level.GetPlayerAvatar();
            Alliance alliance = ObjectManager.GetAlliance(player.GetAllianceId());
            if (player.GetAllianceRole() == 2 || player.GetAllianceRole() == 4)
                if (player.GetAllianceId() == target.GetPlayerAvatar().GetAllianceId())
                {
                    int oldrole = target.GetPlayerAvatar().GetAllianceRole();
                    target.GetPlayerAvatar().SetAllianceRole(m_vRole);
                    if (m_vRole == 2)
                    {
                        player.SetAllianceRole(4);

                        AllianceEventStreamEntry demote = new AllianceEventStreamEntry();
                        demote.SetId(alliance.GetChatMessages().Count + 1);
                        demote.SetSender(player);
                        demote.SetEventType(6);
                        demote.SetAvatarId(player.GetId());
                        demote.SetAvatarName(player.GetAvatarName());

                        alliance.AddChatMessage(demote);

                        AllianceEventStreamEntry promote = new AllianceEventStreamEntry();
                        promote.SetId(alliance.GetChatMessages().Count + 1);
                        promote.SetSender(target.GetPlayerAvatar());
                        promote.SetEventType(5);
                        promote.SetAvatarId(player.GetId());
                        promote.SetAvatarName(player.GetAvatarName());

                        alliance.AddChatMessage(promote);

                        PromoteAllianceMemberOkMessage rup = new PromoteAllianceMemberOkMessage(Client);
                        PromoteAllianceMemberOkMessage rub = new PromoteAllianceMemberOkMessage(target.GetClient());

                        AllianceRoleUpdateCommand p = new AllianceRoleUpdateCommand();
                        AvailableServerCommandMessage pa = new AvailableServerCommandMessage(Client);

                        AllianceRoleUpdateCommand t = new AllianceRoleUpdateCommand();
                        AvailableServerCommandMessage ta = new AvailableServerCommandMessage(target.GetClient());

                        rup.SetID(level.GetPlayerAvatar().GetId());
                        rup.SetRole(4);
                        rub.SetID(target.GetPlayerAvatar().GetId());
                        rub.SetRole(2);

                        t.SetAlliance(alliance);
                        p.SetAlliance(alliance);
                        t.SetRole(2);
                        p.SetRole(4);
                        t.Tick(target);
                        p.Tick(level);

                        ta.SetCommandId(8);
                        pa.SetCommandId(8);
                        ta.SetCommand(t);
                        pa.SetCommand(p);
                        if (ResourcesManager.IsPlayerOnline(target))
                        {
                            ta.Send();
                            rub.Send();
                        }
                        rup.Send();
                        pa.Send();

                        foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                        {
                            Level aplayer = ResourcesManager.GetPlayer(op.GetAvatarId());
                            if (aplayer.GetClient() != null)
                            {
                                AllianceStreamEntryMessage a = new AllianceStreamEntryMessage(aplayer.GetClient());
                                AllianceStreamEntryMessage b = new AllianceStreamEntryMessage(aplayer.GetClient());

                                a.SetStreamEntry(demote);
                                b.SetStreamEntry(promote);

                                a.Send();
                                b.Send();
                            }

                        }
                    }
                    else
                    {
                        AllianceRoleUpdateCommand t = new AllianceRoleUpdateCommand();
                        AvailableServerCommandMessage ta = new AvailableServerCommandMessage(target.GetClient());
                        PromoteAllianceMemberOkMessage ru = new PromoteAllianceMemberOkMessage(target.GetClient());
                        AllianceEventStreamEntry stream = new AllianceEventStreamEntry();

                        stream.SetId(alliance.GetChatMessages().Count + 1);
                        stream.SetSender(target.GetPlayerAvatar());
                        stream.SetAvatarId(player.GetId());
                        stream.SetAvatarName(player.GetAvatarName());
                        if (m_vRole > oldrole)
                            stream.SetEventType(5);
                        else
                            stream.SetEventType(6);
                        
                        t.SetAlliance(alliance);
                        t.SetRole(m_vRole);
                        t.Tick(target);

                        ta.SetCommandId(8);
                        ta.SetCommand(t);

                        ru.SetID(target.GetPlayerAvatar().GetId());
                        ru.SetRole(m_vRole);

                        alliance.AddChatMessage(stream);

                        if (ResourcesManager.IsPlayerOnline(target))
                        {
                            ta.Send();
                            ru.Send();
                        }

                        foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                        {
                            Level aplayer = ResourcesManager.GetPlayer(op.GetAvatarId());
                            if (aplayer.GetClient() != null)
                            {
                                AllianceStreamEntryMessage b = new AllianceStreamEntryMessage(aplayer.GetClient());
                                b.SetStreamEntry(stream);
                                PacketManager.Send(b);
                            }
                        }
                    }
                }
        }
    }
}
