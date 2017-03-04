using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
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

        public LeaveAllianceMessage(Device device, Reader reader) : base(device, reader)
        {

        }

        internal override async void Process()
        {
            try
            {
                ClientAvatar avatar = this.Device.Player.Avatar;
                Alliance alliance = await ObjectManager.GetAlliance(avatar.GetAllianceId());

                if (await avatar.GetAllianceRole() == 2 && alliance.GetAllianceMembers().Count > 1)
                {
                    List<AllianceMemberEntry> members = alliance.GetAllianceMembers();
                    foreach (AllianceMemberEntry player in members.Where(player => player.GetRole() >= 3))
                    {
                        player.SetRole(2);

                        if (ResourcesManager.IsPlayerOnline(await ResourcesManager.GetPlayer(player.GetAvatarId())))
                        {

                            Level l = await ResourcesManager.GetPlayer(player.GetAvatarId());

                            AllianceRoleUpdateCommand c = new AllianceRoleUpdateCommand(l.Client);
                            c.SetAlliance(alliance);
                            c.SetRole(2);
                            c.Tick(l);

                             new AvailableServerCommandMessage(l.Client, c.Handle()).Send();
                        }
                        done = true;
                        break;
                    }
                    if (!done)
                    {
                        int count = alliance.GetAllianceMembers().Count;
                        Random rnd = new Random();
                        int id = rnd.Next(1, count);
                        while (id != this.Device.Player.Avatar.GetId())
                            id = rnd.Next(1, count);
                        int loop = 0;
                        foreach (AllianceMemberEntry player in members)
                        {
                            loop++;
                            if (loop == id)
                            {
                                player.SetRole(2);
                                if (ResourcesManager.IsPlayerOnline(await ResourcesManager.GetPlayer(player.GetAvatarId())))
                                {
                                    Level l2 = await ResourcesManager.GetPlayer(player.GetAvatarId());
                                    AllianceRoleUpdateCommand e = new AllianceRoleUpdateCommand(l2.Client);
                                    e.SetAlliance(alliance);
                                    e.SetRole(2);
                                    e.Tick(l2);

                                    new AvailableServerCommandMessage(l2.Client, e.Handle()).Send();
                                }
                                break;
                            }
                        }
                    }
                }
                LeavedAllianceCommand a = new LeavedAllianceCommand(this.Device);
                a.SetAlliance(alliance);
                a.SetReason(1);

                new AvailableServerCommandMessage(Device, a.Handle()).Send();

                alliance.RemoveMember(avatar.GetId());
                avatar.SetAllianceId(0);

                if (alliance.GetAllianceMembers().Count > 0)
                {
                    AllianceEventStreamEntry eventStreamEntry = new AllianceEventStreamEntry();
                    eventStreamEntry.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    eventStreamEntry.SetSender(avatar);
                    eventStreamEntry.SetEventType(4);
                    eventStreamEntry.SetAvatarId(avatar.GetId());
                    eventStreamEntry.SetAvatarName(avatar.AvatarName);
                    alliance.AddChatMessage(eventStreamEntry);
                    foreach (Level onlinePlayer in ResourcesManager.GetOnlinePlayers())
                        if (onlinePlayer.Avatar.GetAllianceId() == alliance.GetAllianceId())
                        {
                            AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(onlinePlayer.Client);
                            p.SetStreamEntry(eventStreamEntry);
                            p.Send();
                        }
                }
                else
                {
                    DatabaseManager.Single().RemoveAlliance(alliance);
                }
                new LeaveAllianceOkMessage(Device, alliance).Send();
            } catch (Exception) { }
        }
    }
}
