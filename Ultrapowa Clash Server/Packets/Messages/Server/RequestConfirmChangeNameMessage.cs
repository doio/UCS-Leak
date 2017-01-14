using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    class RequestConfirmChangeNameMessage : Message
    {
        public RequestConfirmChangeNameMessage(Packets.Client client, string Name) : base(client)
        {
            SetMessageType(20300);
            m_vName = Name;
        }

        public string m_vName { get; set; }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt64(0);
            data.AddString(m_vName);
            Encrypt(data.ToArray());
        }
    }
}
