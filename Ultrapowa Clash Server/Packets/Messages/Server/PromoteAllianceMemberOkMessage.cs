using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Client;

namespace UCS.Packets.Messages.Server
{
    // Packet 24340
    internal class PromoteAllianceMemberOkMessage : Message
    {
        public PromoteAllianceMemberOkMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24306);
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddInt64(m_vId);
            pack.AddInt32(m_vRole);
            Encrypt(pack.ToArray());
        }
        public void SetID(long id)
        {
            m_vId = id;
        }

        public void SetRole(int role)
        {
            m_vRole = role;
        }

        long m_vId;
        int m_vRole;
    }
}