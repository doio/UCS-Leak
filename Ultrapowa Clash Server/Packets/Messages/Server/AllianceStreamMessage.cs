using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Helpers.List;
using UCS.Logic;
using UCS.Logic.StreamEntry;

namespace UCS.Packets.Messages.Server
{
    // Packet 24311
    internal class AllianceStreamMessage : Message
    {
        readonly Alliance m_vAlliance;

        public AllianceStreamMessage(Device client, Alliance alliance) : base(client)
        {
            this.Identifier = 24311;
            m_vAlliance = alliance;
        }

        internal override void Encode()
        {
            var chatMessages = m_vAlliance.GetChatMessages().ToList();
            this.Data.AddInt(0);
            this.Data.AddInt(chatMessages.Count);
            int count = 0;
            foreach(StreamEntry chatMessage in chatMessages)
            {
                this.Data.AddRange(chatMessage.Encode());
                count++;
                if (count >= 150)
                {
                    break;
                }
            }
        }
    }
}
