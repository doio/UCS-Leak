using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24311
    internal class AllianceStreamMessage : Message
    {
        readonly Alliance m_vAlliance;

        public AllianceStreamMessage(Packets.Client client, Alliance alliance) : base(client)
        {
            SetMessageType(24311);
            m_vAlliance = alliance;
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            var chatMessages = m_vAlliance.GetChatMessages().ToList();
            pack.AddInt32(0); //Unknown
            pack.AddInt32(chatMessages.Count);
            foreach(var chatMessage in chatMessages)
            {
                pack.AddRange(chatMessage.Encode());
            }
            Encrypt(pack.ToArray());
        }
    }
}
