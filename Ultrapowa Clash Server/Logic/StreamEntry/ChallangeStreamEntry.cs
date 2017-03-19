using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Logic.StreamEntry
{
    internal class ChallengeStreamEntry : StreamEntry
    {
        private long m_vAvatarId;
        private string m_vUsername;

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddRange(base.Encode());
            data.AddString(m_vMessage);
            data.AddInt(0);
            return data.ToArray();
        }

        public override int GetStreamEntryType()
        {
            return 12;
        }

        public override void Load(JObject jsonObject)
        {
            base.Load(jsonObject);
            m_vMessage = jsonObject["message"].ToObject<string>();
        }

        public override JObject Save(JObject jsonObject)
        {
            jsonObject = base.Save(jsonObject);
            jsonObject.Add("message", m_vMessage);
            return jsonObject;
        }

        public void SetAvatarId(long id)
        {
            m_vAvatarId = id;
        }

        public void SetUsername(string name)
        {
            m_vUsername = name;
        }
    }
}