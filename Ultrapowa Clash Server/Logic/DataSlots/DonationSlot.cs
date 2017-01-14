using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;

namespace UCS.Logic.DataSlots
{
    internal class DonationSlot
    {
        public long DonatorID;
        public int ID;
        public int Count;
        public int UnitLevel;

        public DonationSlot(long did, int id, int ucount, int ulevel)
        {
            DonatorID = did;
            ID = id;
            Count = ucount;
            UnitLevel = ulevel;
        }

        public void Decode(PacketReader br)            
        {
            DonatorID = br.ReadInt64WithEndian();
            ID = br.ReadInt32WithEndian();
            Count = br.ReadInt32WithEndian();
            UnitLevel = br.ReadInt32WithEndian();
        }

        public byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt64(DonatorID);
            data.AddInt32(ID);
            data.AddInt32(Count);
            data.AddInt32(UnitLevel);
            return data.ToArray();
        }

        public void Load(JObject jsonObject)
        {
            DonatorID = jsonObject["donatorid"].ToObject<long>();
            ID = jsonObject["unitid"].ToObject<int>();
            Count = jsonObject["unitcount"].ToObject<int>();
            UnitLevel = jsonObject["unitlevel"].ToObject<int>();
        }

        public JObject Save(JObject jsonObject)
        {
            jsonObject.Add("donatorid", DonatorID);
            jsonObject.Add("unitid", ID);
            jsonObject.Add("unitcount", Count);
            jsonObject.Add("unitlevel", UnitLevel);
            return jsonObject;
        }
    }
}
