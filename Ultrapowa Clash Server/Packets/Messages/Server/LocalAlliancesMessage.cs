using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24402
    internal class LocalAlliancesMessage : Message
    {
        List<Alliance> m_vAlliances;

        public LocalAlliancesMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24402);
            m_vAlliances = new List<Alliance>();
        }

        public override void Encode()
        {
            List<byte> packet = new List<byte>();
            List<byte> packet1 = new List<byte>();
            int i = 0;

            foreach(var alliance in ObjectManager.GetInMemoryAlliances().OrderByDescending(t => t.GetScore()))
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
            packet.AddInt32(0);       
            packet.AddRange(packet1.ToArray());
            Encrypt(packet.ToArray());
        }
    }
}
