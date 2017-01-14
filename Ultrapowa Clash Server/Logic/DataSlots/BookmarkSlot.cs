using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;

namespace UCS.Logic.DataSlots
{
    internal class BookmarkSlot
    {
        public long Value;

        public BookmarkSlot(long value)
        {
            Value = value;
        }

        public void Decode(PacketReader br)
        {
            Value = br.ReadInt32WithEndian();
        }

        public byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt64(Value);
            return data.ToArray();
        }

        public void Load(JObject jsonObject)
        {
            Value = jsonObject["id"].ToObject<int>();
        }

        public JObject Save(JObject jsonObject)
        {
            jsonObject.Add("id", Value);
            return jsonObject;
        }
    }
}
