using System;
using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Commands.Server
{
    internal class ChangedNameCommand : Command
    {
        public ChangedNameCommand()
        {
        }

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddString(Name);
            data.AddInt64(ID);
            return data.ToArray();
        }

        public string Name { get; set; }
        public long ID { get; set; }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetId(long id)
        {
            ID = id;
        }
    }
}
