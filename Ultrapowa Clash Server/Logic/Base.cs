using System.Collections.Generic;
using System.IO;
using UCS.Helpers;

namespace UCS.Logic
{
    internal class Base
    {
        int m_vUnknown1;

        public Base(int unknown1)
        {
            m_vUnknown1 = unknown1;
        }

        public virtual void Decode(byte[] baseData)
        {
            using (PacketReader br = new PacketReader(new MemoryStream(baseData)))
            {
                m_vUnknown1 = br.ReadInt32WithEndian();
            }
        }

        public virtual byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(m_vUnknown1);
            return data.ToArray();
        }
    }
}
