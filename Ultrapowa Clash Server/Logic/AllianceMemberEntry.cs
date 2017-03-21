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
            AvatarID          = avatarId;
            m_vIsNewMember    = 0;
            Order             = 1;
            PreviousOrder     = 1;
            Role              = 1;
            m_vDonatedTroops  = 0;
            m_vReceivedTroops = 0;
            m_vWarCooldown    = 0;
            WarOptInStatus    = 1;
        }

        readonly int m_vDonatedTroops;
        readonly byte m_vIsNewMember;
        readonly int m_vReceivedTroops;
        readonly int[] m_vRoleTable = { 1, 1, 4, 2, 3 };
        readonly int m_vWarCooldown;
        internal int WarOptInStatus;
        internal long AvatarID;
        internal int Order;
        internal int PreviousOrder;
        internal int Role;

        public static void Decode(byte[] avatarData)
        {
            using (var br = new Reader(avatarData))
            {
            }
        }

        public async Task<byte[]> Encode()
        {
            List<byte> data = new List<byte>();
            Level avatar = await ResourcesManager.GetPlayer(AvatarID);
            data.AddLong(AvatarID);
            if(avatar.Avatar.Username != null)
            {
                data.AddString(avatar.Avatar.Username);
                data.AddInt(Role);
                data.AddInt(avatar.Avatar.Level);
                data.AddInt(avatar.Avatar.League);
                data.AddInt(avatar.Avatar.GetTrophies());
                data.AddInt(m_vDonatedTroops);
                data.AddInt(m_vReceivedTroops);
            }
            else
            {
                data.AddString("Player can't be loaded");
                data.AddInt(Role);
                data.AddInt(1);
                data.AddInt(1);
                data.AddInt(400);
                data.AddInt(0);
                data.AddInt(0);
                avatar.Avatar.AllianceID = 0;
            }          
            data.AddInt(Order);
            data.AddInt(PreviousOrder);
            data.AddInt(m_vIsNewMember);
            data.AddInt(m_vWarCooldown);
            data.AddInt(WarOptInStatus);
            data.Add(1);
            data.AddLong(AvatarID);
            return data.ToArray();
        }

        public bool HasLowerRoleThan(int role)
        {
            bool result = true;
            if (role < m_vRoleTable.Length && Role < m_vRoleTable.Length)
            {
                if (m_vRoleTable[Role] >= m_vRoleTable[role])
                    result = false;
            }
            return result;
        }

        public byte IsNewMember() => m_vIsNewMember;

        public void Load(JObject jsonObject)
        {
            AvatarID = jsonObject["avatar_id"].ToObject<long>();
            Role     = jsonObject["role"].ToObject<int>();
        }

        public JObject Save(JObject jsonObject)
        {
            jsonObject.Add("avatar_id", AvatarID);
            jsonObject.Add("role", Role);
            return jsonObject;
        }

        public void SetAvatarId(long id) => AvatarID = id;       

        public void SetOrder(int order) => Order = order;

        public void ToggleStatus() => WarOptInStatus = WarOptInStatus == 1 ? 0 : 1;

        public void SetPreviousOrder(int order) => PreviousOrder = order;

        public void SetRole(int role) => Role = role;

        public void SetStatus(bool x) => WarOptInStatus = (byte)(x ? 0x01 : 0x00);
    }
}
