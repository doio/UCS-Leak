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

            foreach (var player in ResourcesManager.GetInMemoryLevels().OrderByDescending(t => t.Avatar.GetTrophies()))
            {
                try
                {
                    ClientAvatar pl = player.Avatar;
                    long id = pl.AllianceID;
                    if (i >= 100)
                        break;
                    data.AddLong(pl.UserID);
                    data.AddString(pl.Username);
                    data.AddInt(i + 1);
                    data.AddInt(pl.GetTrophies());
                    data.AddInt(i + 1);
                    data.AddInt(pl.Level);
                    data.AddInt(100);
                    data.AddInt(1);
                    data.AddInt(100);
                    data.AddInt(1);
                    data.AddInt(pl.League);
                    data.AddString(pl.Region.ToUpper());
                    data.AddLong(pl.AllianceID);
                    data.AddInt(1);
                    data.AddInt(1);
                    if (pl.AllianceID > 0)
                    {
                        data.Add(1); // 1 = Have an alliance | 0 = No alliance
                        data.AddLong(pl.AllianceID);
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