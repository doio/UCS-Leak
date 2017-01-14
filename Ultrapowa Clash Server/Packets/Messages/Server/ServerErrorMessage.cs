using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    // Packet 24115
    internal class ServerErrorMessage : Message
    {
        string m_vErrorMessage;

        public ServerErrorMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24115);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddString(m_vErrorMessage);
            Encrypt(data.ToArray());
        }

        public void SetErrorMessage(string message)
        {
            m_vErrorMessage = message;
        }
    }
}
