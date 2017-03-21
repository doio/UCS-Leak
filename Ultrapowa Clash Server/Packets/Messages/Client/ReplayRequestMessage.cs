using System.IO;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14114
    internal class ReplayRequestMessage : Message
    {
        internal long Replay_ID;
        public ReplayRequestMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
            this.Replay_ID = this.Reader.ReadInt64();
            this.Reader.ReadByte();
            this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            new ReplayData(this.Device) { Battle_ID = this.Replay_ID }.Send();
        }
    }
}