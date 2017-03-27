using System.Collections.Generic;
using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Logic
{
    internal class ClientHome : Base
    {
        readonly long m_vId;
        internal string Village;
        int m_vShieldTime;
        int m_vProtectionTime;

        public ClientHome()
        {
        }

        public ClientHome(long id)
        {
            m_vId = id;
        }

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddLong(m_vId);

            data.AddInt(m_vShieldTime); // Shield
            data.AddInt(m_vProtectionTime); // Protection

            data.AddInt(0);
            data.AddCompressed(Village);
            data.AddCompressed("{\"event\":[]}");
            return data.ToArray();
        }

        public void SetHomeJSON(string json) => Village = json;

        public void SetShieldTime(int seconds) => m_vShieldTime = seconds;

        public void SetProtectionTime(int time) => m_vProtectionTime = time;
    }
}
