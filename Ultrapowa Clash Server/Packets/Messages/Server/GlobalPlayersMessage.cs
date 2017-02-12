using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24403
    internal class GlobalPlayersMessage : Message
    {
        public GlobalPlayersMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24403);
        }

        public override async void Encode()
        {
            try
            {
                List<byte> data = new List<byte>();
                List<byte> packet1 = new List<byte>();
                int i = 0;

                foreach (var player in ResourcesManager.GetInMemoryLevels().OrderByDescending(t => t.GetPlayerAvatar().GetScore()))
                {
                    if (player.GetPlayerAvatar().GetAvatarLevel() >= 70)
                    {
                        var pl = player.GetPlayerAvatar();
                        if (i >= 100)
                            break;
                        packet1.AddInt64(pl.GetId());
                        packet1.AddString(pl.GetAvatarName());
                        packet1.AddInt32(i + 1);
                        packet1.AddInt32(pl.GetScore());
                        packet1.AddInt32(i + 1);
                        packet1.AddInt32(pl.GetAvatarLevel());
                        packet1.AddInt32(100);
                        packet1.AddInt32(i);
                        packet1.AddInt32(100);
                        packet1.AddInt32(1);
                        packet1.AddInt32(pl.GetLeagueId());
                        packet1.AddString(pl.GetUserRegion().ToUpper());
                        packet1.AddInt64(pl.GetId());
                        packet1.AddInt32(1);
                        packet1.AddInt32(1);
                        if (pl.GetAllianceId() > 0)
                        {
                            packet1.Add(1); // 1 = Have an alliance | 0 = No alliance
                            packet1.AddInt64(pl.GetAllianceId());
                            Alliance _Alliance = await ObjectManager.GetAlliance(pl.GetAllianceId());
                            packet1.AddString(_Alliance.GetAllianceName());
                            packet1.AddInt32(_Alliance.GetAllianceBadgeData());
                        }
                        else
                            packet1.Add(0);
                        i++;
                    }
                }

                data.AddInt32(i);
                data.AddRange(packet1);
                data.AddInt32(i);
                data.AddRange(packet1);

                data.AddInt32((int)TimeSpan.FromDays(7).TotalSeconds);
                data.AddInt32(DateTime.Now.Year);
                data.AddInt32(DateTime.Now.Month);
                data.AddInt32(DateTime.Now.Year);
                data.AddInt32(DateTime.Now.Month - 1);
                Encrypt(data.ToArray());
            } catch (Exception) { }
        }
    }
}
