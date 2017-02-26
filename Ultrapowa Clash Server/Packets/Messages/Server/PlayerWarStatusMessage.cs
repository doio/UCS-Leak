using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    internal class PlayerWarStatusMessage : Message
    {
        public PlayerWarStatusMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24111);
        }

        private int Status;

        public override void Encode()
        {
            List<byte> _Data = new List<byte>();
            _Data.AddInt32(14);
            _Data.AddInt32(Status);
            _Data.AddInt32(0);
            Encrypt(_Data.ToArray());
        }

        public void SetStatus(int i)
        {
            Status = i;
        }
    }
}
