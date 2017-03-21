using System.Collections.Generic;
using UCS.Helpers;
using UCS.Helpers.List;
using System;
using UCS.Core.Checker;

namespace UCS.Logic
{
    internal class ClientHome 
    {
        readonly long m_vId;
        string village;
        int m_vShieldTime;
        int m_vProtectionTime;

        public ClientHome()
        {
        }

        public ClientHome(long id)
        {
            m_vId = id;
        }

        public byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddLong(m_vId);

            data.AddInt(m_vShieldTime); // Shield
            data.AddInt(m_vProtectionTime); // Protection

            data.AddInt(1);
            data.AddCompressed(village);
            data.AddCompressed(DirectoryChecker._Events);
            return data.ToArray();
        }

        public string GetHomeJSON() => village;

        public void SetHomeJSON(string json) => village = json;

        public void SetShieldTime(int seconds) => m_vShieldTime = seconds;

        public int Shield() => m_vShieldTime;

        public void SetProtectionTime(int time) => m_vProtectionTime = time;

        public int Guard() => m_vProtectionTime;
    }
}
