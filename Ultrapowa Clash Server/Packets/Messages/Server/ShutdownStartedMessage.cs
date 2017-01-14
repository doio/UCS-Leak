using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    // Packet 20161
    internal class ShutdownStartedMessage : Message
    {
        int m_vCode;

        public ShutdownStartedMessage(Packets.Client client) : base(client)
        {
            SetMessageType(20161);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(m_vCode);
            Encrypt(data.ToArray());
        }

        public void SetCode(int code)
        {
            m_vCode = code;
        }
    }
}
