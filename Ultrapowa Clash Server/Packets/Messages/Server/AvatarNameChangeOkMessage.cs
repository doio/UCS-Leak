using System.Collections.Generic;
using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Packets.Messages.Server
{
    // Packet 24111
    internal class UsernameChangeOkMessage : Message
    {
        public UsernameChangeOkMessage(Device client) : base(client)
        {
            this.Identifier = 24111;
            Username = "NoNameYet";
        }

        internal string Username;

        internal override void Encode()
        {
            this.Data.AddInt(3);
            this.Data.AddString(this.Username);
            this.Data.AddInt(1);
            this.Data.AddInt(-1);
        }
    }
}