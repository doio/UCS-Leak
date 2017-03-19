using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24403
    internal class GlobalPlayersMessage : Message
    {
        public GlobalPlayersMessage(Device client) : base(client)
        {
            this.Identifier = 24403;
        }

        internal override async void Encode()
        {
            List<byte> packet1 = new List<byte>();
            int i = 0;

            foreach (var player in ResourcesManager.GetInMemoryLevels().OrderByDescending(t => t.Avatar.GetTrophies()))
            {
                if (player.Avatar.Level >= 70)
                {
                    try
                    {
                        ClientAvatar pl = player.Avatar;
                        if (i >= 100)
                            break;
                        packet1.AddLong(pl.UserID);
                        packet1.AddString(pl.Username);
                        packet1.AddInt(i + 1);
                        packet1.AddInt(pl.GetTrophies());
                        packet1.AddInt(i + 1);
                        packet1.AddInt(pl.Level);
                        packet1.AddInt(100);
                        packet1.AddInt(i);
                        packet1.AddInt(100);
                        packet1.AddInt(1);
                        packet1.AddInt(pl.League);
                        packet1.AddString(pl.Region.ToUpper());
                        packet1.AddLong(pl.UserID);
                        packet1.AddInt(1);
                        packet1.AddInt(1);
                        if (pl.AllianceID > 0)
                        {
                            packet1.Add(1); // 1 = Have an alliance | 0 = No alliance
                            packet1.AddLong(pl.AllianceID);
                            Alliance _Alliance = await ObjectManager.GetAlliance(pl.AllianceID);
                            packet1.AddString(_Alliance.GetAllianceName());
                            packet1.AddInt(_Alliance.GetAllianceBadgeData());
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

            this.Data.AddInt(i);
            this.Data.AddRange(packet1);
            this.Data.AddInt(i);
            this.Data.AddRange(packet1);

            this.Data.AddInt((int) TimeSpan.FromDays(7).TotalSeconds);
            this.Data.AddInt(DateTime.Now.Year);
            this.Data.AddInt(DateTime.Now.Month);
            this.Data.AddInt(DateTime.Now.Year);
            this.Data.AddInt(DateTime.Now.Month - 1);
        }
    }
}