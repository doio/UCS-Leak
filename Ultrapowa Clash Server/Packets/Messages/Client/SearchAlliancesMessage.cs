using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14324
    internal class SearchAlliancesMessage : Message
    {
        public SearchAlliancesMessage(Device device, Reader reader) : base(device, reader)
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

        internal override void Decode()
        {
            this.m_vWarFrequency = this.Reader.ReadInt32();
            this.m_vAllianceOrigin = this.Reader.ReadInt32();
            this.m_vMinimumAllianceMembers = this.Reader.ReadInt32();
            this.m_vMaximumAllianceMembers = this.Reader.ReadInt32();
            this.m_vAllianceScore = this.Reader.ReadInt32();
            this.m_vShowOnlyJoinableAlliances = this.Reader.ReadByte();
            this.Reader.ReadInt32();
            this.m_vMinimumAllianceLevel = this.Reader.ReadInt32();

        }

        internal override void Process()
        {
            if (m_vSearchString.Length > 15)
            {
                ResourcesManager.DisconnectClient(Device);
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
                        if (alliances[i].m_vAllianceName.Contains(m_vSearchString, StringComparison.OrdinalIgnoreCase))
                        {
                            joinableAlliances.Add(alliances[i]);
                            j++;
                        }
                        i++;
                    }
                }
                joinableAlliances = joinableAlliances.ToList();

                AllianceListMessage p = new AllianceListMessage(Device);
                p.SetAlliances(joinableAlliances);
                p.SetSearchString(m_vSearchString);
                p.Send();
            }
        }
    }
}