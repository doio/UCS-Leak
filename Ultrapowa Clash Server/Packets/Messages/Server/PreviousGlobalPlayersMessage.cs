using System;
using System.Collections.Generic;
using UCS.Core;
using UCS.Helpers;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    internal class PreviousGlobalPlayersMessage : Message
    {
        public PreviousGlobalPlayersMessage(Device client) : base(client)
        {
            this.Identifier = 24405;
        }

        internal override async void Encode()
        {
            try
            {
                List<byte> packet1 = new List<byte>();

                int i = 1;
                foreach (var player in ResourcesManager.GetOnlinePlayers())
                {
                    ClientAvatar pl = player.Avatar;
                    packet1.AddLong(pl.GetId());
                    packet1.AddString(pl.AvatarName);
                    packet1.AddInt(i);
                    packet1.AddInt(pl.GetScore());
                    packet1.AddInt(i);
                    packet1.AddInt(pl.GetAvatarLevel());
                    packet1.AddInt(100);
                    packet1.AddInt(1);
                    packet1.AddInt(100);
                    packet1.AddInt(1);
                    packet1.AddInt(pl.GetLeagueId());
                    packet1.AddString("EN");
                    packet1.AddLong(pl.GetId());
                    packet1.AddInt(1);
                    packet1.AddInt(1);
                    if (pl.GetAllianceId() > 0)
                    {
                        packet1.Add(1); // 1 = Have an alliance | 0 = No alliance
                        packet1.AddLong(pl.GetAllianceId());
                        Alliance _Alliance = await ObjectManager.GetAlliance(pl.GetAllianceId());
                        packet1.AddString(_Alliance.GetAllianceName());
                        packet1.AddInt(_Alliance.GetAllianceBadgeData());
                    }
                    else
                        packet1.Add(0);
                    if (i >= 101)
                        break;
                    i++;
                }
                this.Data.AddInt(i - 1);
                this.Data.AddRange(packet1);
                this.Data.AddInt(DateTime.Now.Month - 1);
                this.Data.AddInt(DateTime.Now.Year);
            }
            catch (Exception)
            {
            }
        }
    }
}
