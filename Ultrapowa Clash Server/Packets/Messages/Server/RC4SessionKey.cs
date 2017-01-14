using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    //Packet 20000
    internal class RC4SessionKey : Message
    {
        public RC4SessionKey(Packets.Client client) : base(client)
        {
            SetMessageType(20000);
            Key = Utils.CreateRandomByteArray();
        }
        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddByteArray(Key);
            pack.AddInt32(1);
            Encrypt(pack.ToArray());
        }
        public byte[] Key { get; set; }
    }
}