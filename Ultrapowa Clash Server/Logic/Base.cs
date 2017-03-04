using System.Collections.Generic;
using System.IO;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Helpers.List;

namespace UCS.Logic
{
    internal class Base
    {
        public Base()
        {
        }

        public virtual void Decode(byte[] baseData)
        {
        }

        public virtual byte[] Encode()
        {
            List<byte> data = new List<byte>();
            return data.ToArray();
        }
    }
}
