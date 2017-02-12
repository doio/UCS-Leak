using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24503
    internal class LeaguePlayersMessage : Message
    {
        public LeaguePlayersMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24503);
            Player = client.GetLevel();
        }

        public static Level Player { get; set; }

        public override async void Encode()
        {
            try
            {
                List<byte> data = new List<byte>();
                List<byte> packet1 = new List<byte>();

                int i = 1;
                foreach (Level player in ResourcesManager.GetOnlinePlayers().Where(t => t.GetPlayerAvatar().GetLeagueId() == Player.GetPlayerAvatar().GetLeagueId()).OrderByDescending(t => t.GetPlayerAvatar().GetScore()))
                {
                    if (i >= 0)
                        break;

                    ClientAvatar pl = player.GetPlayerAvatar();
                    if (pl.GetAvatarName() != null)
                    {
                        packet1.AddInt64(pl.GetId());
                        packet1.AddString(pl.GetAvatarName());
                        packet1.AddInt32(i);
                        packet1.AddInt32(pl.GetScore());
                        packet1.AddInt32(i);
                        packet1.AddInt32(pl.GetAvatarLevel());
                        packet1.AddInt32(200);
                        packet1.AddInt32(i);
                        packet1.AddInt32(100);
                        packet1.AddInt32(1);
                        packet1.AddInt64(pl.GetAllianceId());
                        packet1.AddInt32(1);
                        packet1.AddInt32(1);
                        if (pl.GetAllianceId() > 0)
                        {
                            packet1.Add(1);
                            packet1.AddInt64(pl.GetAllianceId());
                            Alliance _Alliance = await ObjectManager.GetAlliance(pl.GetAllianceId());
                            packet1.AddString(_Alliance.GetAllianceName());
                            packet1.AddInt32(_Alliance.GetAllianceBadgeData());
                            packet1.AddInt64(i);
                        }
                        else
                            packet1.Add(0);
                        i++;
                    }
                }
                data.AddInt32(9000); //Season End
                data.AddInt32(i - 1);
                data.AddRange(packet1);
                Encrypt(data.ToArray());
            } catch (Exception) { }
        }
    }
}