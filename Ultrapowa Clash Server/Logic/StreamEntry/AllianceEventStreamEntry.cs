using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Logic.StreamEntry
{
    internal class AllianceEventStreamEntry : StreamEntry
    {
        long m_vAvatarId;
        string m_vAvatarName;
        int m_vEventType;

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddRange(base.Encode());
            data.AddInt(m_vEventType);
            data.AddLong(m_vAvatarId);
            data.AddString(m_vAvatarName);
            return data.ToArray();
        }

        public override int GetStreamEntryType() => 4;
       
        //event id's
        // 1 = kicked from clan
        // 2 = accecpted to clan
        // 3 = join clan
        // 4 = leave clan
        // 5 = promote
        // 6 = demote
        // 7 = start clan war search
        // 8 = cancel clan war search
        // 9 = clan war oponnent not found
        // 10 = update clan setting

        public override void Load(JObject jsonObject)
        {
            m_vAvatarName = jsonObject["avatar_name"].ToObject<string>();
            m_vEventType  = jsonObject["event_type"].ToObject<int>();
            m_vAvatarId   = jsonObject["avatar_id"].ToObject<long>();
        }

        public override JObject Save(JObject jsonObject)
        {
            jsonObject.Add("avatar_name", m_vAvatarName);
            jsonObject.Add("event_type", m_vEventType);
            jsonObject.Add("avatar_id", m_vAvatarId);
            return jsonObject;
        }

        public void SetAvatarId(long id) => m_vAvatarId = id;

        public void SetAvatarName(string name) => m_vAvatarName = name;

        public void SetEventType(int type) => m_vEventType = type;
    }
}
