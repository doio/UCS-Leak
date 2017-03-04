using UCS.Helpers;
using UCS.Helpers.List;

namespace UCS.Packets.Messages.Server
{
    //Packet 20000
    internal class RC4SessionKey : Message
    {
        public RC4SessionKey(Device client) : base(client)
        {
            this.Identifier = 20000;
        }
        internal override void Encode()
        {
            this.Data.AddByteArray(Utils.CreateRandomByteArray());
            this.Data.AddInt(1);
        }
    }
}