using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    // Packet 24111
    internal class AvailableServerCommandMessage : Message
    {
        public AvailableServerCommandMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24111);
        }

        Command m_vCommand;
        int m_vServerCommandId;

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddInt32(m_vServerCommandId);
            pack.AddRange(m_vCommand.Encode());
            Encrypt(pack.ToArray());
        }

        public void SetCommand(Command c)
        {
            m_vCommand = c;
        }

        public void SetCommandId(int id)
        {
            m_vServerCommandId = id;
        }
    }
}