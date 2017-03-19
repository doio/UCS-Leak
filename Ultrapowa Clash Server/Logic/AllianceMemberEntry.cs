using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.Helpers;
using System.Threading.Tasks;
using UCS.Helpers.Binary;
using UCS.Helpers.List;

namespace UCS.Logic
{
    internal class AllianceMemberEntry
    { 
        public AllianceMemberEntry(long avatarId)
        {
            m_vAvatarId       = avatarId;
            m_vIsNewMember    = 0;
            m_vOrder          = 1;
            m_vPreviousOrder  = 1;
            m_vRole           = 1;
            m_vDonatedTroops  = 200;
            m_vReceivedTroops = 100;
            m_vWarCooldown    = 0;
            m_vWarOptInStatus = 1;
        }

        readonly int m_vDonatedTroops;
        readonly byte m_vIsNewMember;
        readonly int m_vReceivedTroops;
        readonly int[] m_vRoleTable = { 1, 1, 4, 2, 3 };
        readonly int m_vWarCooldown;
        int m_vWarOptInStatus;
        long m_vAvatarId;
        int m_vOrder;
        int m_vPreviousOrder;
        int m_vRole;

        public static void Decode(byte[] avatarData)
        {
            using (var br = new Reader(avatarData))
            {
            }
        }

        public static int GetDonations() => 150;

        public async Task<byte[]> Encode()
        {
            List<byte> data = new List<byte>();
            Level avatar = await ResourcesManager.GetPlayer(m_vAvatarId);
            data.AddLong(m_vAvatarId);
            if(avatar.Avatar.Username != null)
            {
                data.AddString(avatar.Avatar.Username);
                data.AddInt(m_vRole);
                data.AddInt(avatar.Avatar.Level);
                data.AddInt(avatar.Avatar.League);
                data.AddInt(avatar.Avatar.GetTrophies());
                data.AddInt(m_vDonatedTroops);
                data.AddInt(m_vReceivedTroops);
            }
            else
            {
                data.AddString("Player can't be loaded");
                data.AddInt(m_vRole);
                data.AddInt(1);
                data.AddInt(1);
                data.AddInt(400);
                data.AddInt(0);
                data.AddInt(0);
                avatar.Avatar.AllianceID = 0;
            }          
            data.AddInt(m_vOrder);
            data.AddInt(m_vPreviousOrder);
            data.AddInt(m_vIsNewMember);
            data.AddInt(m_vWarCooldown);
            data.AddInt(m_vWarOptInStatus);
            data.Add(1);
            data.AddLong(m_vAvatarId);
            return data.ToArray();
        }

        public long GetAvatarId() => m_vAvatarId;

        public int GetOrder() => m_vOrder;

        public int GetPreviousOrder() => m_vPreviousOrder;

        public int GetRole() => m_vRole;

        public int GetStatus() => m_vWarOptInStatus;

        public bool HasLowerRoleThan(int role)
        {
            bool result = true;
            if (role < m_vRoleTable.Length && m_vRole < m_vRoleTable.Length)
            {
                if (m_vRoleTable[m_vRole] >= m_vRoleTable[role])
                    result = false;
            }
            return result;
        }

        public byte IsNewMember() => m_vIsNewMember;

        public void Load(JObject jsonObject)
        {
            m_vAvatarId = jsonObject["avatar_id"].ToObject<long>();
            m_vRole = jsonObject["role"].ToObject<int>();
        }

        public JObject Save(JObject jsonObject)
        {
            jsonObject.Add("avatar_id", m_vAvatarId);
            jsonObject.Add("role", m_vRole);
            return jsonObject;
        }

        public void SetAvatarId(long id)
        {
            m_vAvatarId = id;
        }

        public void SetOrder(int order) => m_vOrder = order;

        public void ToggleStatus() => m_vWarOptInStatus = m_vWarOptInStatus == 1 ? 0 : 1;

        public void SetPreviousOrder(int order) => m_vPreviousOrder = order;

        public void SetRole(int role) => m_vRole = role;

        public void SetStatus(bool x) => m_vWarOptInStatus = (byte)(x ? 0x01 : 0x00);
    }
}
