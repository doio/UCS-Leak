using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;

namespace UCS.Logic.DataSlots
{
    internal class TroopDataSlot
    {
        public TroopDataSlot(Data d, int value, int value1)
        {
            Data = d;
            Value = value;
            Value1 = value1;
        }

        public Data Data;
        public int Value;
        public int Value1;

        public void Decode(PacketReader br)
        {
            Data = br.ReadDataReference();
            Value = br.ReadInt32WithEndian();
            Value1 = br.ReadInt32WithEndian();
        }

        public byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(Data.GetGlobalID());
            data.AddInt32(Value);
            data.AddInt32(Value1);
            return data.ToArray();
        }

        public void Load(JObject jsonObject)
        {
            Data = CSVManager.DataTables.GetDataById(jsonObject["global_id"].ToObject<int>());
            Value = jsonObject["count"].ToObject<int>();
            Value1 = jsonObject["level"].ToObject<int>();
        }

        public JObject Save(JObject jsonObject)
        {
            jsonObject.Add("global_id", Data.GetGlobalID());
            jsonObject.Add("count", Value);
            jsonObject.Add("level", Value1);
            return jsonObject;
        }
    }
}
