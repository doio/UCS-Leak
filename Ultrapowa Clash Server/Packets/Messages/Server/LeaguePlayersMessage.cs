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
            int i = 0;

            this.Data.AddInt(9000); //Season End
            this.Data.AddInt(i);

            foreach (Level player in ResourcesManager.GetOnlinePlayers().Where(t => t.Avatar.GetLeagueId() == this.Device.Player.Avatar.GetLeagueId()).OrderByDescending(t => t.Avatar.GetScore()))
            {
                if (i >= 50)
                    break;

                ClientAvatar pl = player.Avatar;
                if (pl.AvatarName != null)
                {
                    try
                    {
                        this.Data.AddLong(pl.GetId());
                        this.Data.AddString(pl.AvatarName);
                        this.Data.AddInt(i + 1);
                        this.Data.AddInt(pl.GetScore());
                        this.Data.AddInt(i + 1);
                        this.Data.AddInt(pl.GetAvatarLevel());
                        this.Data.AddInt(pl.GetDonated());
                        this.Data.AddInt(i + 1);
                        this.Data.AddInt(pl.GetReceived());
                        this.Data.AddInt(1);
                        this.Data.AddLong(pl.GetAllianceId());
                        this.Data.AddInt(1);
                        this.Data.AddInt(1);
                        if (pl.GetAllianceId() > 0)
                        {
                            this.Data.Add(1);
                            this.Data.AddLong(pl.GetAllianceId());
                            Alliance _Alliance = await ObjectManager.GetAlliance(pl.GetAllianceId());
                            this.Data.AddString(_Alliance.GetAllianceName());
                            this.Data.AddInt(_Alliance.GetAllianceBadgeData());
                            this.Data.AddLong(i + 1);
                        }
                        else
                        {
                            this.Data.Add(0);
                        }
                        i++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}