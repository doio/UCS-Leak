using System.Collections.Generic;
using UCS.Logic.AvatarStreamEntry;

namespace UCS.Packets.Messages.Server
{
    // Packet 24412
    internal class AvatarStreamEntryMessage : Message
    {
        AvatarStreamEntry m_vAvatarStreamEntry;

        public AvatarStreamEntryMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24412);
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();   
            pack.AddRange(m_vAvatarStreamEntry.Encode());
            Encrypt(pack.ToArray());
        }

        public void SetAvatarStreamEntry(AvatarStreamEntry entry)
        {
            m_vAvatarStreamEntry = entry;
        }
    }
}