using System.Collections.Generic;
using System.IO;
using UCS.Utilities.ZLib;

namespace UCS.Packets.Messages.Server
{
    // Packet 24224
    internal class ReplayData : Message
    {
        public ReplayData(Packets.Client client) : base(client)
        {
            SetMessageType(24114);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            string text = File.ReadAllText("replay-json.txt");
            data.AddRange(ZlibStream.CompressString(text));
            Encrypt(data.ToArray());
        }
    }
}