using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Helpers.List;

namespace UCS.Packets.Messages.Server
{
    // Packet 24402
    internal class LocalAlliancesMessage : Message
    {
        public LocalAlliancesMessage(Device client) : base(client)
        {
            this.Identifier = 24402;
        }

        internal override void Encode()
        {
            List<byte> packet1 = new List<byte>();
            int i = 0;

            foreach(var alliance in ObjectManager.GetInMemoryAlliances().OrderByDescending(t => t.GetTrophies()))
            {
                if (i >= 100)
                    break;
                packet1.AddLong(alliance.AllianceID);
                packet1.AddString(alliance.GetAllianceName());
                packet1.AddInt(i + 1);
                packet1.AddInt(alliance.GetTrophies());
                packet1.AddInt(i + 1);
                packet1.AddInt(alliance.GetAllianceBadgeData());
                packet1.AddInt(alliance.GetAllianceMembers().Count);
                packet1.AddInt(0);
                packet1.AddInt(alliance.GetAllianceLevel());
                i++;
            }
            this.Data.AddInt(0);       
            this.Data.AddRange(packet1.ToArray());
        }
    }
}
