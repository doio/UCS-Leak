using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24404
    internal class LocalPlayersMessage : Message
    {
        public LocalPlayersMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24404);
        }

        public override void Encode()
        {
            List<byte> packet = new List<byte>();
            List<byte> data = new List<byte>();
            var i = 0;

            foreach(var player in ResourcesManager.GetInMemoryLevels().OrderByDescending(t => t.GetPlayerAvatar().GetScore()))
            {
                ClientAvatar pl = player.GetPlayerAvatar();
                long id = pl.GetAllianceId();
                if (i >= 100)
                    break;
                data.AddInt64(pl.GetId());
                data.AddString(pl.GetAvatarName());
                data.AddInt32(i + 1);
                data.AddInt32(pl.GetScore());
                data.AddInt32(i + 1);
                data.AddInt32(pl.GetAvatarLevel());
                data.AddInt32(100);
                data.AddInt32(1);
                data.AddInt32(100);
                data.AddInt32(1);
                data.AddInt32(pl.GetLeagueId());
                data.AddString(pl.GetUserRegion().ToUpper());
                data.AddInt64(pl.GetAllianceId());
                data.AddInt32(1);
                data.AddInt32(1);
                if (pl.GetAllianceId() > 0)
                {
                    data.Add(1); // 1 = Have an alliance | 0 = No alliance
                    data.AddInt64(pl.GetAllianceId());
                    data.AddString(ObjectManager.GetAlliance(id).GetAllianceName());
                    data.AddInt32(ObjectManager.GetAlliance(id).GetAllianceBadgeData());
                }
                else
                    data.Add(0);
                i++;
            }

            packet.AddInt32(i);
            packet.AddRange(data.ToArray());
            Encrypt(packet.ToArray());
        }
    }
}
