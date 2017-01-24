using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24113
    internal class VisitedHomeDataMessage : Message
    {
        public VisitedHomeDataMessage(Packets.Client client, Level ownerLevel, Level visitorLevel) : base(client)
        {
            SetMessageType(24113);
            m_vOwnerLevel = ownerLevel;
            m_vVisitorLevel = visitorLevel;
        }

        public override void Encode()
        {

            List<byte> data = new List<byte>();
            ClientHome ownerHome = new ClientHome(m_vOwnerLevel.GetPlayerAvatar().GetId());
            ownerHome.SetShieldTime(m_vOwnerLevel.GetPlayerAvatar().GetShieldTime);
            ownerHome.SetProtectionTime(m_vOwnerLevel.GetPlayerAvatar().GetProtectionTime);
            ownerHome.SetHomeJSON(m_vOwnerLevel.SaveToJSON());

            data.AddInt32(-1);
            data.AddInt32((int)TimeSpan.FromSeconds(100).TotalSeconds);
            data.AddRange(ownerHome.Encode());
            data.AddRange(m_vOwnerLevel.GetPlayerAvatar().Encode());
            data.AddInt32(0);
            data.Add(1);
            data.AddRange(m_vVisitorLevel.GetPlayerAvatar().Encode());

            Encrypt(data.ToArray());
        }

        readonly Level m_vOwnerLevel;
        readonly Level m_vVisitorLevel;
    }
}
