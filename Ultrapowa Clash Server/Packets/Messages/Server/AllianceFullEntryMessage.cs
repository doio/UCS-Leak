using System.Collections.Generic;
using System.Threading.Tasks;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24324
    internal class AllianceFullEntryMessage : Message
    {
        readonly Alliance m_vAlliance;

        public AllianceFullEntryMessage(Packets.Client client, Alliance alliance) : base(client)
        {
            SetMessageType(24324);
            m_vAlliance = alliance;
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            var allianceMembers = m_vAlliance.GetAllianceMembers(); 
			
			
            pack.AddString(m_vAlliance.GetAllianceDescription());
            pack.AddInt32(0);//War state:
                             // 0 - Not started
                             // 1 - Search enemy (old war type)
                             // 2 - Search enemy (new war type)
                             // 3 - Unknown
                             // 4 - Preparation day
                             // 5 - Battle day
                             // 6 - War end 
            pack.AddInt32(0);//Unknown

            pack.Add(0);       //0 if no war
            //pack.AddInt64(WarID);
            pack.Add(0);
            pack.AddInt32(0);
            pack.AddRange(m_vAlliance.EncodeFullEntry());

            Encrypt(pack.ToArray());
        }
    }
}
