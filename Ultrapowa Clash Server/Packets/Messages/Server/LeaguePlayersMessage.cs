using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24503
    internal class LeaguePlayersMessage : Message
    {
        public LeaguePlayersMessage(Device client) : base(client)
        {
            this.Identifier = 24503;
        }

        internal override async void Encode()
        {
            List<byte> packet1 = new List<byte>();

            int i = 1;
            foreach (Level player in  ResourcesManager.GetOnlinePlayers() .Where(t => t.Avatar.GetLeagueId() == this.Device.Player.Avatar.GetLeagueId()) .OrderByDescending(t => t.Avatar.GetScore()))
            {
                if (i >= 51)
                    break;

                ClientAvatar pl = player.Avatar;
                if (pl.AvatarName != null)
                {
                    try
                    {
                        packet1.AddLong(pl.GetId());
                        packet1.AddString(pl.AvatarName);
                        packet1.AddInt(i);
                        packet1.AddInt(pl.GetScore());
                        packet1.AddInt(i);
                        packet1.AddInt(pl.GetAvatarLevel());
                        packet1.AddInt(200);
                        packet1.AddInt(i);
                        packet1.AddInt(100);
                        packet1.AddInt(1);
                        packet1.AddLong(pl.GetAllianceId());
                        packet1.AddInt(1);
                        packet1.AddInt(1);
                        if (pl.GetAllianceId() > 0)
                        {
                            packet1.Add(1);
                            packet1.AddLong(pl.GetAllianceId());
                            Alliance _Alliance = await ObjectManager.GetAlliance(pl.GetAllianceId());
                            packet1.AddString(_Alliance.GetAllianceName());
                            packet1.AddInt(_Alliance.GetAllianceBadgeData());
                            packet1.AddLong(i);
                        }
                        else
                            packet1.Add(0);
                        i++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            this.Data.AddInt(9000); //Season End
            this.Data.AddInt(i - 1);
            this.Data.AddRange(packet1);

        }
    }
}