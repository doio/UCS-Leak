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
        private int m_vStatus;

        // TODO

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddInt32(14);
            pack.AddInt32(0); //
            pack.AddInt32(0);
            Encrypt(pack.ToArray());
        }

        public void SetStatus(int s)
        {
            m_vStatus = s;
        }
    }
}
