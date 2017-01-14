using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;

namespace UCS.Logic.StreamEntry
{
    internal class ChallangeStreamEntry : StreamEntry
    {
        long m_vAvatarId;
        string m_vAvatarName;
        string Message;

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddRange(base.Encode());
            data.AddString(Message);
            data.AddInt32(0);
            return data.ToArray();
        }

        public override int GetStreamEntryType() => 12;

        public override void Load(JObject jsonObject)
        {
            Message = jsonObject["Message"].ToObject<string>();
        }

        public override JObject Save(JObject jsonObject)
        {
            jsonObject.Add("Message", Message);
            return jsonObject;
        }

        public void SetAvatarId(long id) => m_vAvatarId = id;

        public void SetAvatarName(string name) => m_vAvatarName = name;

        public void SetMessage(string message) => Message = message;
    }
}
