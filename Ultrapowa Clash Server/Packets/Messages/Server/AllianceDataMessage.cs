using System.Collections.Generic;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24301
    internal class AllianceDataMessage : Message
    {
        readonly Alliance m_vAlliance;

        public AllianceDataMessage(Packets.Client client, Alliance alliance) : base(client)
        {
            SetMessageType(24301);
            m_vAlliance = alliance;
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            var allianceMembers = m_vAlliance.GetAllianceMembers();

            pack.AddRange(m_vAlliance.EncodeFullEntry());
            pack.AddString(m_vAlliance.GetAllianceDescription());
            pack.AddInt32(0);
            pack.Add(0);  
            pack.AddInt32(0);
            pack.Add(0);

            pack.AddInt32(allianceMembers.Count);

            foreach(AllianceMemberEntry m in allianceMembers)
            {
				pack.AddRange(m.Encode());
            }

            pack.AddInt32(0);
            pack.AddInt32(32);
			Encrypt(pack.ToArray());
        }
    }
}
