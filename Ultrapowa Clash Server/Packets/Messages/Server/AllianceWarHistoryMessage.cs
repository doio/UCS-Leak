using System;
using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24338
    internal class AllianceWarHistoryMessage : Message
    {
        readonly Alliance m_vHomeAlliance;
        public AllianceWarHistoryMessage(Packets.Client client, Alliance home) : base(client)
        {
            SetMessageType(24338);
            m_vHomeAlliance = home;
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();

            data.AddInt32(1);

            data.AddInt64(m_vHomeAlliance.GetAllianceId()); // 1 Alliance ID
            data.AddString(m_vHomeAlliance.GetAllianceName()); // 1 Alliance Name
            data.AddInt32(m_vHomeAlliance.GetAllianceBadgeData()); // 1 Alliance Badge
            data.AddInt32(m_vHomeAlliance.GetAllianceLevel()); // 1 Alliance Level

            data.AddInt64(9999); // 2 Alliance ID
            data.AddString("Ultrapowa"); // 2 Alliance Name
            data.AddInt32(0); // 2 Alliance Badge
            data.AddInt32(1); // 2 Alliance Level

            data.AddInt32(9999); // 1 Stars
            data.AddInt32(0); // 2 Stars

            data.AddInt32(0); // 1 Village Destroyed
            data.AddInt32(100); // 2 Village Destroyed

            data.AddInt32(0); // 1 Unknown
            data.AddInt32(0); // 2 Unknown

            data.AddInt32(100); // Attack Used
            data.AddInt32(4000); // XP Earned

            data.AddInt64(515631654); // War ID
            data.AddInt64(40); // Members Count

            data.AddInt32(1); // War Won Count

            data.Add(99);
            data.AddInt32((int) TimeSpan.FromDays(1).TotalSeconds);
            data.AddInt64((int) (TimeSpan.FromDays(1).TotalSeconds - TimeSpan.FromDays(0.5).TotalSeconds));

            Encrypt(data.ToArray());
        }
    }
}
