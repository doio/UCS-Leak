using System.Collections.Generic;
using UCS.Core.Crypto;
using UCS.Utilities.Blake2b;
using UCS.Helpers;
using UCS.Packets.Messages.Client;

namespace UCS.Packets.Messages.Server
{
    // Packet 20100
    internal class HandshakeSuccess : Message
    {
        private byte[] _sessionKey;
        private static readonly Hasher Blake = Blake2B.Create(new Blake2BConfig { OutputSizeInBytes = 24 });

        public HandshakeSuccess(Packets.Client client, SessionRequest cka) : base(client)
        {
            SetMessageType(20100);
            Blake.Init();
            Blake.Update(Key.Crypto.PrivateKey);
            _sessionKey = Blake.Finish();
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddByteArray(_sessionKey);
            SetData(pack.ToArray());
        }
    }
}
