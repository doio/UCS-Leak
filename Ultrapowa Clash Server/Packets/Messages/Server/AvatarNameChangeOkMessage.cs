using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    // Packet 24111
    internal class AvatarNameChangeOkMessage : Message
    {
        public AvatarNameChangeOkMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24111);
            m_vAvatarName = "NoNameYet";
        }

        string m_vAvatarName;

        public override void Encode()
        {
            List<byte> pack = new List<byte>();

            pack.AddInt32(3);
            pack.AddString(m_vAvatarName);
            pack.AddInt32(1);
            pack.AddInt32(-1);

            Encrypt(pack.ToArray());
        }

        public string GetAvatarName() => m_vAvatarName;

        public void SetAvatarName(string name)
        {
            m_vAvatarName = name;
        }
    }
}