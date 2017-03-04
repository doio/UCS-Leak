using System;
using System.IO;
using System.Text;
using UCS.Core;
using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Messages.Client
{
    // Packet 14100
    internal class AttackResultMessage : Message
    {
        public AttackResultMessage(Device device, Reader reader) : base(device, reader)
        {

        }
    }
}