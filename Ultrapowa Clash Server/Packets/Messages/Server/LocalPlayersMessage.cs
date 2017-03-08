using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24404
    internal class LocalPlayersMessage : Message
    {
        public LocalPlayersMessage(Device client) : base(client)
        {
            this.Identifier = 24404;
        }

        internal override async void Encode()
        {
            List<byte> data = new List<byte>();
            var i = 0;

            foreach (var player in ResourcesManager.GetInMemoryLevels().OrderByDescending(t => t.Avatar.GetScore()))
            {
                try
                {
                    ClientAvatar pl = player.Avatar;
                    long id = pl.GetAllianceId();
                    if (i >= 100)
                        break;
                    data.AddLong(pl.GetId());
                    data.AddString(pl.AvatarName);
                    data.AddInt(i + 1);
                    data.AddInt(pl.GetScore());
                    data.AddInt(i + 1);
                    data.AddInt(pl.GetAvatarLevel());
                    data.AddInt(100);
                    data.AddInt(1);
                    data.AddInt(100);
                    data.AddInt(1);
                    data.AddInt(pl.GetLeagueId());
                    data.AddString(pl.Region.ToUpper());
                    data.AddLong(pl.GetAllianceId());
                    data.AddInt(1);
                    data.AddInt(1);
                    if (pl.GetAllianceId() > 0)
                    {
                        data.Add(1); // 1 = Have an alliance | 0 = No alliance
                        data.AddLong(pl.GetAllianceId());
                        Alliance _Alliance = await ObjectManager.GetAlliance(id);
                        data.AddString(_Alliance.GetAllianceName());
                        data.AddInt(_Alliance.GetAllianceBadgeData());
                    }
                    else
                        data.Add(0);
                    i++;
                }
                catch (Exception)
                {
                }
            }

            this.Data.AddInt(i);
            this.Data.AddRange(data.ToArray());
        }
    }
}