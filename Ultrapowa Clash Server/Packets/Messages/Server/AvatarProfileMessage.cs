using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24334
    internal class AvatarProfileMessage : Message
    {
        Level m_vLevel;

        public AvatarProfileMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24334);
        }

        public override void Encode()
        {
            var pack = new List<byte>();
            var ch = new ClientHome(m_vLevel.GetPlayerAvatar().GetId());
            ch.SetHomeJSON(m_vLevel.SaveToJSON());

            pack.AddRange(m_vLevel.GetPlayerAvatar().Encode());
            pack.AddCompressedString(ch.GetHomeJSON());

            pack.AddInt32(0); //Donated
            pack.AddInt32(0); //Received
            pack.AddInt32(0); //War Cooldown

            pack.AddInt32(0); //Unknown
            pack.Add(0); //Unknown


            Encrypt(pack.ToArray());
        }

        public void SetLevel(Level level)
        {
            m_vLevel = level;
        }
    }
}
