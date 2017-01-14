using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Packets.Messages.Server
{
    // Packet 24331
    internal class AllianceWarDataMessage : Message
    {
        public AllianceWarDataMessage(Packets.Client client) : base(client)
        {
            SetMessageType(24331);
        }

        public override void Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(0);
            data.AddInt32(0);

            data.AddInt64(1); // Team ID
            data.AddString("Ultrapowa");
            data.AddInt32(0);
            data.AddInt32(1);
            data.Add(0);
            data.AddRange(new List<byte> { 1, 2, 3, 4 });
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);

            data.AddInt64(1); // Team ID
            data.AddString("Ultrapowa");
            data.AddInt32(0);
            data.AddInt32(1);
            data.Add(0);
            data.AddRange(new List<byte> { 1, 2, 3, 4 });
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);

            data.AddInt64(11);

            data.AddInt32(0);
            data.AddInt32(0);

            data.AddInt32(1);
            data.AddInt32(3600);
            data.AddInt64(1);
            data.AddInt64(1);
            data.AddInt64(2);
            data.AddInt64(2);

            data.AddString("Ultra");
            data.AddString("Powa");

            data.AddInt32(2);
            data.AddInt32(1);
            data.AddInt32(50);

            data.AddInt32(0);

            data.AddInt32(8);
            data.AddInt32(0);
            data.AddInt32(0);
            data.Add(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);

            Encrypt(data.ToArray());
        }
    }
}
