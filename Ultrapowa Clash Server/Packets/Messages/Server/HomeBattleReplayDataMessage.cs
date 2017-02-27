using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24114
    internal class HomeBattleReplayDataMessage : Message
    {
        public HomeBattleReplayDataMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24114);
        }

        public override void Encode()
        {
            List<byte> _Data = new List<byte>();
            /*_Data.Add(1);
            _Data.AddCompressedString(data);*/
            Encrypt(_Data.ToArray());
        }
    }
}