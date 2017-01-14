using System;
using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24107
    internal class EnemyHomeDataMessage : Message
    {
        public EnemyHomeDataMessage(Packets.Client client, Level ownerLevel, Level visitorLevel) : base(client)
        {
            SetMessageType(24107);
            m_vOwnerLevel = ownerLevel;
            m_vVisitorLevel = visitorLevel;
        }

        public override void Encode()
        {
            m_vOwnerLevel.GetPlayerAvatar().State = ClientAvatar.UserState.PVP;
            List<byte> data = new List<byte>();
            ClientHome ch = new ClientHome(m_vOwnerLevel.GetPlayerAvatar().GetId());
            ch.SetShieldTime(m_vOwnerLevel.GetPlayerAvatar().RemainingShieldTime);
            ch.SetHomeJSON(m_vOwnerLevel.SaveToJSON());

            data.AddInt32((int)TimeSpan.FromSeconds(100).TotalSeconds);
            data.AddInt32(-1);
            data.AddInt32((int)Client.GetLevel().GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            data.AddRange(ch.Encode());
            data.AddRange(m_vOwnerLevel.GetPlayerAvatar().Encode());
            data.AddRange(m_vVisitorLevel.GetPlayerAvatar().Encode());
            data.AddInt32(3);
            data.AddInt32(0);
            data.Add(0);

            Encrypt(data.ToArray());
        }

        readonly Level m_vOwnerLevel;
        readonly Level m_vVisitorLevel;
    }
}
