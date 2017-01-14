using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24111
    internal class LeaveAllianceOkMessage : Message
    {
        public LeaveAllianceOkMessage(Packets.Client client, Alliance alliance) : base(client)
        {
            SetMessageType(24111);

            m_vServerCommandType = 0x02;
            m_vAlliance = alliance;
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddInt32(m_vServerCommandType);
            pack.AddInt64(m_vAlliance.GetAllianceId());
            pack.AddInt32(1); // 1 = leave, 2 = kick
            pack.AddInt32(-1);
            Encrypt(pack.ToArray());
        }

        readonly Alliance m_vAlliance;
        readonly int m_vServerCommandType;
    }
}