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
            try
            {
                int i = 0;

                this.Data.AddInt(9000); //Season End
                this.Data.AddInt(i);

                foreach (Level player in ResourcesManager.GetOnlinePlayers().Where(t => t.Avatar.League == this.Device.Player.Avatar.League).OrderByDescending(t => t.Avatar.GetTrophies()))
                {
                    if (i >= 50)
                        break;

                    ClientAvatar pl = player.Avatar;
                    if (pl.Username != null)
                    {
                        try
                        {
                            this.Data.AddLong(pl.UserID);
                            this.Data.AddString(pl.Username);
                            this.Data.AddInt(i + 1);
                            this.Data.AddInt(pl.GetTrophies());
                            this.Data.AddInt(i + 1);
                            this.Data.AddInt(pl.Level);
                            this.Data.AddInt(pl.Donations);
                            this.Data.AddInt(i + 1);
                            this.Data.AddInt(pl.Received);
                            this.Data.AddInt(1);
                            this.Data.AddLong(pl.AllianceID);
                            this.Data.AddInt(1);
                            this.Data.AddInt(1);
                            if (pl.AllianceID > 0)
                            {
                                this.Data.Add(1);
                                this.Data.AddLong(pl.AllianceID);
                                Alliance _Alliance = await ObjectManager.GetAlliance(pl.AllianceID);
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
            catch (Exception)
            {
            }
        }
    }
}