using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;

namespace UCS.Logic
{
    internal class DataSlot
    {
        public DataSlot(Data d, int value)
        {
            Data = d;
            Value = value;
        }

        public Data Data;
        public int Value;

        public void Decode(PacketReader br)
        {
            Data = br.ReadDataReference();
            Value = br.ReadInt32WithEndian();
        }

        public byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(Data.GetGlobalID());
            data.AddInt32(Value);
            return data.ToArray();
        }

        public void Load(JObject jsonObject)
        {
            Data = CSVManager.DataTables.GetDataById(jsonObject["global_id"].ToObject<int>());
            Value = jsonObject["value"].ToObject<int>();
        }

        public JObject Save(JObject jsonObject)
        {
            jsonObject.Add("global_id", Data.GetGlobalID());
            jsonObject.Add("value", Value);
            return jsonObject;
        }

    }
}
