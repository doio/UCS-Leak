using System;
using System.IO;
using System.Text;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 14100
    internal class AttackResultMessage : Message
    {
        public AttackResultMessage(Packets.Client client, PacketReader br) : base(client, br)
        {

        }

        public override void Decode()
        {
            // TODO
            // Console.WriteLine("Packet Attack Result : " + Encoding.UTF8.GetString(GetData()));
        }

        public override void Process(Level level)
        {
        }
    }
}