using System.Collections.Generic;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14303
    internal class AskForJoinableAlliancesListMessage : Message
    {
        const int m_vAllianceLimit = 40;

        public AskForJoinableAlliancesListMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            List<Alliance> alliances = ObjectManager.GetInMemoryAlliances();
            List<Alliance> joinableAlliances = new List<Alliance>();
            int i = 0;
            int j = 0;
            while (j < m_vAllianceLimit && i < alliances.Count)
            {
                if (alliances[i].GetAllianceMembers().Count != 0 && !alliances[i].IsAllianceFull())
                {
                    joinableAlliances.Add(alliances[i]);
                    j++;
                }
                i++;
            }
            joinableAlliances = joinableAlliances.ToList();

            JoinableAllianceListMessage p = new JoinableAllianceListMessage(Client);
            p.SetJoinableAlliances(joinableAlliances);
            PacketManager.Send(p);
        }
    }
}
