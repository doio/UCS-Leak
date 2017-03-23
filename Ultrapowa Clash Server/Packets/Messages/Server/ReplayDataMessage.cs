using System;
using System.IO;
using Newtonsoft.Json;
using UCS.Core;
using UCS.Helpers.List;

namespace UCS.Packets.Messages.Server
{
    // Packet 24224
    internal class ReplayData : Message
    {
        internal long Battle_ID = 0;

        public ReplayData(Device client) : base(client)
        {
            this.Identifier = 24114;
        }
        static JsonSerializerSettings Settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

        internal override void Encode()
        {
            string Replay = JsonConvert.SerializeObject(DatabaseManager.Single().GetBattle(Battle_ID), Settings);
            this.Data.AddCompressed(Replay, false);
        }
    }
}