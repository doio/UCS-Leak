using System;
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
    // Packet 14324
    internal class SearchAlliancesMessage : Message
    {
        public SearchAlliancesMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        const int m_vAllianceLimit = 40;
        int m_vAllianceOrigin;
        int m_vAllianceScore;
        int m_vMaximumAllianceMembers;
        int m_vMinimumAllianceLevel;
        int m_vMinimumAllianceMembers;
        string m_vSearchString;
        byte m_vShowOnlyJoinableAlliances;
        int m_vWarFrequency;

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                m_vSearchString = br.ReadScString();
                m_vWarFrequency = br.ReadInt32WithEndian();
                m_vAllianceOrigin = br.ReadInt32WithEndian();
                m_vMinimumAllianceMembers = br.ReadInt32WithEndian();
                m_vMaximumAllianceMembers = br.ReadInt32WithEndian();
                m_vAllianceScore = br.ReadInt32WithEndian();
                m_vShowOnlyJoinableAlliances = br.ReadByte();
                br.ReadInt32WithEndian();
                m_vMinimumAllianceLevel = br.ReadInt32WithEndian();
            }
        }

        public override void Process(Level level)
        {
            if (m_vSearchString.Length > 15)
            {
                ResourcesManager.DisconnectClient(Client);
            }
            else
            {
                List<Alliance> alliances = ObjectManager.GetInMemoryAlliances();

                List<Alliance> joinableAlliances = new List<Alliance>();
                int i = 0;
                int j = 0;
                while (j < m_vAllianceLimit && i < alliances.Count)
                {
                    if (alliances[i].GetAllianceMembers().Count != 0)
                    {
                        if (alliances[i].GetAllianceName().Contains(m_vSearchString, StringComparison.OrdinalIgnoreCase))
                        {
                            joinableAlliances.Add(alliances[i]);
                            j++;
                        }
                        i++;
                    }
                }
                joinableAlliances = joinableAlliances.ToList();

                AllianceListMessage p = new AllianceListMessage(Client);
                p.SetAlliances(joinableAlliances);
                p.SetSearchString(m_vSearchString);
                PacketProcessor.Send(p);
            }
        }
    }
}