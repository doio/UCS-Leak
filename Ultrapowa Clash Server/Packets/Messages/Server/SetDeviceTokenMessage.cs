using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 20113
    internal class SetDeviceTokenMessage : Message
    {
        readonly Level level;

        public SetDeviceTokenMessage(Packets.Client client) : base(client)
        {
            SetMessageType(20113);
            level = client.GetLevel();
        }

        public override void Encode()
        {
            List<byte> pack = new List<byte>();
            pack.AddString(level.GetPlayerAvatar().GetUserToken());
            Encrypt(pack.ToArray());
        }
    }
}