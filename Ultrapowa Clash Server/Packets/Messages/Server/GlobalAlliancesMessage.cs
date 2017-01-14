using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24401
    internal class GlobalAlliancesMessage : Message
    {
        List<Alliance> m_vAlliances;

        public GlobalAlliancesMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24401);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            List<byte> packet1 = new List<byte>();
            int i = 0;

            foreach (var alliance in ObjectManager.GetInMemoryAlliances().OrderByDescending(t => t.GetScore()))
            {
                if (i >= 100)
                    break;
                packet1.AddInt64(alliance.GetAllianceId());
                packet1.AddString(alliance.GetAllianceName());
                packet1.AddInt32(i + 1);
                packet1.AddInt32(alliance.GetScore());
                packet1.AddInt32(i + 1);
                packet1.AddInt32(alliance.GetAllianceBadgeData());
                packet1.AddInt32(alliance.GetAllianceMembers().Count);
                packet1.AddInt32(0);
                packet1.AddInt32(alliance.GetAllianceLevel());
                i++;
            }

            data.AddInt32(i);
            data.AddRange(packet1);

            data.AddInt32((int) TimeSpan.FromDays(1).TotalSeconds);
            data.AddInt32(3);
            data.AddInt32(50000);
            data.AddInt32(30000);
            data.AddInt32(15000);
            Encrypt(data.ToArray());
        }
    }
}
