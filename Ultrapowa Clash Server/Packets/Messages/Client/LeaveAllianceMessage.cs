using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Commands.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14308
    internal class LeaveAllianceMessage : Message
    {
        public static bool done;

        public LeaveAllianceMessage(Packets.Client client, PacketReader br) : base(client, br)
        {

        }

        public override void Decode()
        {

        }

        public override void Process(Level level)
        {
            ClientAvatar avatar = level.GetPlayerAvatar();
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());

            if (avatar.GetAllianceRole() == 2 && alliance.GetAllianceMembers().Count > 1)
            {
                List<AllianceMemberEntry> members = alliance.GetAllianceMembers();
                foreach (AllianceMemberEntry player in members.Where(player => player.GetRole() >= 3))
                {
                    player.SetRole(2);

                    if (ResourcesManager.IsPlayerOnline(ResourcesManager.GetPlayer(player.GetAvatarId())))
                    {
                        AllianceRoleUpdateCommand c = new AllianceRoleUpdateCommand();
                        c.SetAlliance(alliance);
                        c.SetRole(2);
                        c.Tick(level);

                        AvailableServerCommandMessage d = new AvailableServerCommandMessage(ResourcesManager.GetPlayer(player.GetAvatarId()).GetClient());
                        d.SetCommandId(8);
                        d.SetCommand(c);
                        d.Send();
                    }
                    done = true;
                    break;
                }
                if (!done)
                {
                    int count = alliance.GetAllianceMembers().Count;
                    Random rnd = new Random();
                    int id = rnd.Next(1, count);
                    while (id != level.GetPlayerAvatar().GetId())
                        id = rnd.Next(1, count);
                    int loop = 0;
                    foreach (AllianceMemberEntry player in members)
                    {
                        loop++;
                        if (loop == id)
                        {
                            player.SetRole(2);
                            if (ResourcesManager.IsPlayerOnline(ResourcesManager.GetPlayer(player.GetAvatarId())))
                            {
                                AllianceRoleUpdateCommand e = new AllianceRoleUpdateCommand();
                                e.SetAlliance(alliance);
                                e.SetRole(2);
                                e.Tick(level);

                                AvailableServerCommandMessage f = new AvailableServerCommandMessage(ResourcesManager.GetPlayer(player.GetAvatarId()).GetClient());
                                f.SetCommandId(8);
                                f.SetCommand(e);
                                f.Send();
                            }
                            break;
                        }
                    }
                }
            }
            LeavedAllianceCommand a = new LeavedAllianceCommand();
            a.SetAlliance(alliance);
            a.SetReason(1);

            AvailableServerCommandMessage b = new AvailableServerCommandMessage(Client);
            b.SetCommandId(2);
            b.SetCommand(a);
            b.Send();

            alliance.RemoveMember(avatar.GetId());
            avatar.SetAllianceId(0);

            if (alliance.GetAllianceMembers().Count > 0)
            {
                AllianceEventStreamEntry eventStreamEntry = new AllianceEventStreamEntry();
                eventStreamEntry.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                eventStreamEntry.SetSender(avatar);
                eventStreamEntry.SetEventType(4);
                eventStreamEntry.SetAvatarId(avatar.GetId());
                eventStreamEntry.SetAvatarName(avatar.GetAvatarName());
                alliance.AddChatMessage(eventStreamEntry);
                foreach (Level onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == alliance.GetAllianceId())
                    {
                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(onlinePlayer.GetClient());
                        p.SetStreamEntry(eventStreamEntry);
                        p.Send();
                    }
            }
            else
            {
                DatabaseManager.Single().RemoveAlliance(alliance);
            }
            new LeaveAllianceOkMessage(Client, alliance).Send();
        }
    }
}
