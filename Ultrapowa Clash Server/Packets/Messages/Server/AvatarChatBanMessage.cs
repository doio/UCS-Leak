using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    internal class AvatarChatBanMessage : Message
    {
        public int m_vCode = 86400;

        public AvatarChatBanMessage(Packets.Client client) : base(client)
        {
            SetMessageType(20118);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(m_vCode);
            Encrypt(data.ToArray());
        }

        public void SetBanPeriod(int code)
        {
            m_vCode = code;
        }
    }
}
