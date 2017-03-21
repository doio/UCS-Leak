using System.Collections.Generic;
using UCS.Helpers;
using UCS.Helpers.List;
using System;
using UCS.Core.Checker;

namespace UCS.Logic
{
    internal class ClientHome
    {
        internal long Id;
        internal string Village;
        internal int ShieldTime;
        internal int GuardTime;
        internal bool Amical;

        public ClientHome()
        {
        }

        public ClientHome(long id)
        {
            this.Id = id;
        }

        public byte[] Encode()
        {
            List<byte> data = new List<byte>();
            if (Amical) // Amical Battle
            {
                data.AddLong(this.Id);
                data.AddLong(this.Id);
            }
            data.AddLong(this.Id);

            data.AddInt(this.ShieldTime); // Shield
            data.AddInt(this.GuardTime); // Protection

            data.AddInt(1);
            data.AddCompressed(this.Village);
            data.AddCompressed(DirectoryChecker._Events);
            return data.ToArray();
        }
    }
}
