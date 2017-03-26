using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.Logic.StreamEntry;
using System.Threading.Tasks;
using UCS.Helpers.List;

namespace UCS.Logic
{
    internal class Alliance
    {
        const int m_vMaxAllianceMembers    = 50;
        const int m_vMaxChatMessagesNumber = 30;
        internal readonly Dictionary<long, AllianceMemberEntry> m_vAllianceMembers;
        internal readonly List<StreamEntry.StreamEntry> m_vChatMessages;
        internal int m_vAllianceBadgeData;
        internal string m_vAllianceDescription;
        internal int m_vAllianceExperience;
        internal long m_vAllianceId;
        internal int m_vAllianceLevel;
        internal string m_vAllianceName;
        internal int m_vAllianceOrigin;
        internal int m_vAllianceType;
        internal int m_vDrawWars;
        internal int m_vLostWars;
        internal int m_vRequiredScore;
        internal int m_vScore;
        internal int m_vWarFrequency;
        internal byte m_vWarLogPublic;
        internal int m_vWonWars;
        internal byte m_vFriendlyWar;

        public Alliance()
        {
            m_vChatMessages    = new List<StreamEntry.StreamEntry>();
            m_vAllianceMembers = new Dictionary<long, AllianceMemberEntry>();
        }

        public Alliance(long id)
        {
            Random r               = new Random();
            m_vAllianceId          = id;
            m_vAllianceName        = "Default";
            m_vAllianceDescription = "Default";
            m_vAllianceBadgeData   = 0;
            m_vAllianceType        = 0;
            m_vRequiredScore       = 0;
            m_vWarFrequency        = 0;
            m_vAllianceOrigin      = 32000001;
            m_vScore               = 0;
            m_vAllianceExperience  = r.Next(100, 5000);
            m_vAllianceLevel       = r.Next(6, 10);
            m_vWonWars             = r.Next(200, 500);
            m_vLostWars            = r.Next(100, 300);
            m_vDrawWars            = r.Next(100, 800);
            m_vChatMessages        = new List<StreamEntry.StreamEntry>();
            m_vAllianceMembers     = new Dictionary<long, AllianceMemberEntry>();
        }

        public void AddAllianceMember(AllianceMemberEntry entry) => m_vAllianceMembers.Add(entry.AvatarId, entry);

        public void AddChatMessage(StreamEntry.StreamEntry message)
        {
            while (m_vChatMessages.Count >= m_vMaxChatMessagesNumber)
            {
                m_vChatMessages.RemoveAt(0);
            }
            m_vChatMessages.Add(message);
        }

        public byte[] EncodeFullEntry()
        {
            List<byte> data = new List<byte>();
            data.AddLong(m_vAllianceId);
            data.AddString(m_vAllianceName);
            data.AddInt(m_vAllianceBadgeData);
            data.AddInt(m_vAllianceType);
            data.AddInt(m_vAllianceMembers.Count);
            data.AddInt(m_vScore);
            data.AddInt(m_vRequiredScore);
            data.AddInt(m_vWonWars);
            data.AddInt(m_vLostWars);
            data.AddInt(m_vDrawWars);
            data.AddInt(20000001);
            data.AddInt(m_vWarFrequency);
            data.AddInt(m_vAllianceOrigin);
            data.AddInt(m_vAllianceExperience);
            data.AddInt(m_vAllianceLevel);
            data.AddInt(0);
            data.AddInt(0);
            data.Add(m_vWarLogPublic);
            data.Add(m_vFriendlyWar);
            return data.ToArray();
        }

        public byte[] EncodeHeader()
        {
            List<byte> data = new List<byte>();
            data.AddLong(m_vAllianceId);
            data.AddString(m_vAllianceName);
            data.AddInt(m_vAllianceBadgeData);
            data.Add(0);
            data.AddInt(m_vAllianceLevel);
            data.AddInt(1);
            data.AddInt(-1);
            return data.ToArray();
        }

        public List<AllianceMemberEntry> GetAllianceMembers() => m_vAllianceMembers.Values.ToList();

        public List<StreamEntry.StreamEntry> GetChatMessages() => m_vChatMessages;

        public bool IsAllianceFull() => m_vAllianceMembers.Count >= m_vMaxAllianceMembers;

        public async void LoadFromJSON(string jsonString)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);
                m_vAllianceId = jsonObject["alliance_id"].ToObject<long>();
                m_vAllianceName = jsonObject["alliance_name"].ToObject<string>();
                m_vAllianceBadgeData = jsonObject["alliance_badge"].ToObject<int>();
                m_vAllianceType = jsonObject["alliance_type"].ToObject<int>();
                m_vRequiredScore = jsonObject["required_score"].ToObject<int>();
                m_vAllianceDescription = jsonObject["description"].ToObject<string>();
                m_vAllianceExperience = jsonObject["alliance_experience"].ToObject<int>();
                m_vAllianceLevel = jsonObject["alliance_level"].ToObject<int>();
                m_vWarLogPublic = jsonObject["war_log_public"].ToObject<byte>();
                m_vFriendlyWar = jsonObject["friendly_war"].ToObject<byte>();
                m_vWonWars = jsonObject["won_wars"].ToObject<int>();
                m_vLostWars = jsonObject["lost_wars"].ToObject<int>();
                m_vDrawWars = jsonObject["draw_wars"].ToObject<int>();
                m_vWarFrequency = jsonObject["war_frequency"].ToObject<int>();
                m_vAllianceOrigin = jsonObject["alliance_origin"].ToObject<int>();
                JArray jsonMembers = (JArray)jsonObject["members"];
                foreach (JToken jToken in jsonMembers)
                {
                    JObject jsonMember = (JObject)jToken;
                    long id = jsonMember["avatar_id"].ToObject<long>();
                    Level pl = await ResourcesManager.GetPlayer(id);
                    AllianceMemberEntry member = new AllianceMemberEntry(id);
                    m_vScore = m_vScore + pl.Avatar.GetScore();
                    member.Load(jsonMember);
                    m_vAllianceMembers.Add(id, member);
                }
                m_vScore = m_vScore / 2;
                JArray jsonMessages = (JArray)jsonObject["chatMessages"];
                if (jsonMessages != null)
                {
                    foreach (JToken jToken in jsonMessages)
                    {
                        JObject jsonMessage = (JObject)jToken;
                        StreamEntry.StreamEntry se = new StreamEntry.StreamEntry();
                        if (jsonMessage["type"].ToObject<int>() == 1)
                            se = new TroopRequestStreamEntry();
                        else if (jsonMessage["type"].ToObject<int>() == 2)
                            se = new ChatStreamEntry();
                        else if (jsonMessage["type"].ToObject<int>() == 3)
                            se = new InvitationStreamEntry();
                        else if (jsonMessage["type"].ToObject<int>() == 4)
                            se = new AllianceEventStreamEntry();
                        else if (jsonMessage["type"].ToObject<int>() == 5)
                            se = new ShareStreamEntry();
                        else { }
                        se.Load(jsonMessage);
                        m_vChatMessages.Add(se);
                    }
                }
            }
            catch (Exception) { }
        }

        public void RemoveMember(long avatarId) => m_vAllianceMembers.Remove(avatarId);

        public string SaveToJSON()
        {
            var jsonData = new JObject();
            jsonData.Add("alliance_id", m_vAllianceId);
            jsonData.Add("alliance_name", m_vAllianceName);
            jsonData.Add("alliance_badge", m_vAllianceBadgeData);
            jsonData.Add("alliance_type", m_vAllianceType);
            jsonData.Add("score", m_vScore);
            jsonData.Add("required_score", m_vRequiredScore);
            jsonData.Add("description", m_vAllianceDescription);
            jsonData.Add("alliance_experience", m_vAllianceExperience);
            jsonData.Add("alliance_level", m_vAllianceLevel);
            jsonData.Add("war_log_public", m_vWarLogPublic);
            jsonData.Add("friendly_war", m_vFriendlyWar);
            jsonData.Add("won_wars", m_vWonWars);
            jsonData.Add("lost_wars", m_vLostWars);
            jsonData.Add("draw_wars", m_vDrawWars);
            jsonData.Add("war_frequency", m_vWarFrequency);
            jsonData.Add("alliance_origin", m_vAllianceOrigin);
            JArray jsonMembersArray = new JArray();
            foreach (AllianceMemberEntry member in m_vAllianceMembers.Values)
            {
                JObject jsonObject = new JObject();
                member.Save(jsonObject);
                jsonMembersArray.Add(jsonObject);
            }
            jsonData.Add("members", jsonMembersArray);
            JArray jsonMessageArray = new JArray();
            foreach (StreamEntry.StreamEntry message in m_vChatMessages)
            {
                JObject jsonObject = new JObject();
                message.Save(jsonObject);
                jsonMessageArray.Add(jsonObject);
            }
            jsonData.Add("chatMessages", jsonMessageArray);
            return JsonConvert.SerializeObject(jsonData);
        }

        public void SetAllianceBadgeData(int data) => m_vAllianceBadgeData = data;

        public void SetAllianceDescription(string description) => m_vAllianceDescription = description;

        public void SetAllianceLevel(int level) => m_vAllianceLevel = level;

        public void SetAllianceName(string name) => m_vAllianceName = name;

        public void SetAllianceOrigin(int origin) => m_vAllianceOrigin = origin;

        public void SetAllianceType(int status) => m_vAllianceType = status;

        public void SetRequiredScore(int score) => m_vRequiredScore = score;

        public void SetWarFrequency(int frequency) => m_vWarFrequency = frequency;

        public void SetWarPublicStatus(byte log) => m_vWarLogPublic = log;

        public void SetFriendlyWar(byte log) => m_vFriendlyWar = log;

        public void SetWarAndFriendlytStatus(byte status)
        {
            if (status == 1)
            {
                SetWarPublicStatus(1);
            }
            else if (status == 2)
            {
                SetFriendlyWar(1);
            }
            else if (status == 3)
            {
                SetWarPublicStatus(1);
                SetFriendlyWar(1);
            }
            else if (status == 0)
            {
                SetWarPublicStatus(0);
                SetFriendlyWar(0);
            }
        }
    }
}
