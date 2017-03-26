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
    internal class ChallangeStreamEntry : StreamEntry
    {
        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddRange(base.Encode());
            data.AddString(m_vMessage);
            data.AddInt(0);
            return data.ToArray();
        }

        public override int GetStreamEntryType() => 12;

        public override void Load(JObject jsonObject)
        {
            m_vMessage = jsonObject["Message"].ToObject<string>();
        }

        public override JObject Save(JObject jsonObject)
        {
            jsonObject.Add("Message", m_vMessage);
            return jsonObject;
        }
    }
}
