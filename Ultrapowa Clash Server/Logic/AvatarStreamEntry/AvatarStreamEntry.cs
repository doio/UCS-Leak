using System;
using System.Collections.Generic;
using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Logic.AvatarStreamEntry
{
    internal class AvatarStreamEntry
    {
        public AvatarStreamEntry()
        {
            m_vCreationTime = DateTime.UtcNow;
        }

        DateTime m_vCreationTime;
        int m_vId;
        byte m_vIsNew;
        long m_vSenderId;
        int m_vSenderLeagueId;
        int m_vSenderLevel;
        string m_vSenderName;

        public virtual byte[] Encode()
        {
            var data = new List<byte>();
            data.AddInt(GetStreamEntryType());
            data.AddLong(m_vId);
            data.Add(1);
            data.AddLong(m_vSenderId);
            data.AddString(m_vSenderName);
            data.AddInt(m_vSenderLevel);
            data.AddInt(m_vSenderLeagueId);
            data.AddInt(10);
            data.Add(m_vIsNew);
            return data.ToArray();
        }

        public int GetAgeSeconds() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds -
        (int)m_vCreationTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public int GetId() => m_vId;

        public long GetSenderAvatarId() => m_vSenderId;

        public int GetSenderLevel() => m_vSenderLevel;

        public string GetSenderName() => m_vSenderName;

        public virtual int GetStreamEntryType() => -1;

        public byte IsNew() => m_vIsNew;

        public void SetAvatar(ClientAvatar avatar)
        {
            m_vSenderId = avatar.UserId;
            m_vSenderName = avatar.AvatarName;
            m_vSenderLevel = avatar.m_vAvatarLevel;
            m_vSenderLeagueId = avatar.m_vLeagueId;
        }

        public void SetId(int id) => m_vId = id;

        public void SetIsNew(byte isNew) => m_vIsNew = isNew;

        public void SetSenderAvatarId(long id) => m_vSenderId = id;

        public void SetSenderLeagueId(int id) => m_vSenderLeagueId = id;

        public void SetSenderLevel(int level) => m_vSenderLevel = level;

        public void SetSenderName(string name) => m_vSenderName = name;
    }
}
