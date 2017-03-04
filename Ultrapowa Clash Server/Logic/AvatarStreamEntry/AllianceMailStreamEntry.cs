using System.Collections.Generic;
using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Logic.AvatarStreamEntry
{
    internal class AllianceMailStreamEntry : AvatarStreamEntry
    {
        int m_vAllianceBadgeData;
        long m_vAllianceId;
        string m_vAllianceName;
        string m_vMessage;
        long m_vSenderId;

        public override byte[] Encode()
        {
            var data = new List<byte>();
            data.AddRange(base.Encode());
            data.AddString(m_vMessage);
            data.Add(1);
            data.AddLong(m_vSenderId);
            data.AddLong(m_vAllianceId);
            data.AddString(m_vAllianceName);
            data.AddInt(m_vAllianceBadgeData);
            return data.ToArray();
        }

        public string GetMessage() => m_vMessage;

        public override int GetStreamEntryType() => 6;

        public void SetAllianceBadgeData(int data) => m_vAllianceBadgeData = data;

        public void SetAllianceId(long id) => m_vAllianceId = id;

        public void SetAllianceName(string name) => m_vAllianceName = name;

        public void SetMessage(string message) => m_vMessage = message;

        public void SetSenderId(long id) => m_vSenderId = id;
    }
}
