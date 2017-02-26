using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;

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
            int count = 0;
            foreach(StreamEntry chatMessage in chatMessages)
            {
                if (chatMessage != null)
                {
                    if (chatMessage.Encode() != null)
                    {
                        pack.AddRange(chatMessage.Encode());
                        count++;
                        if (count >= 150)
                        {
                            break;
                        }
                    }
                }
            }
            Encrypt(pack.ToArray());
        }
    }
}
