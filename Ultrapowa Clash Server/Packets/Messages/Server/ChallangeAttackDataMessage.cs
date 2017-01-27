using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    internal class ChallangeAttackDataMessage : Message
    {
        private readonly Level m_vOwnerLevel;
        private readonly Level m_vVisitorLevel;

        public ChallangeAttackDataMessage(Packets.Client client, Level level) : base(client)
        {
            SetMessageType(24107);
            m_vOwnerLevel = level;
            m_vVisitorLevel = client.GetLevel();
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddRange(new byte[]
            {
                0x00, 0x00, 0x00, 0xF0,
                0xFF, 0xFF, 0xFF, 0xFF,
                0x54, 0xCE, 0x5C, 0x4A
            });
            ClientHome ch = new ClientHome(m_vOwnerLevel.GetPlayerAvatar().GetId());
            ch.SetHomeJSON(m_vOwnerLevel.SaveToJSON());
            data.AddRange(ch.Encode());
            data.AddRange(m_vOwnerLevel.GetPlayerAvatar().Encode());
            data.AddRange(m_vVisitorLevel.GetPlayerAvatar().Encode());
            data.AddRange(new byte[]
            {
                0x00, 0x00, 0x00, 0x03, 0x00
            });
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt64(0);
            data.AddInt64(0);
            data.AddInt64(0);
            Encrypt(data.ToArray());
        }
    }
}
