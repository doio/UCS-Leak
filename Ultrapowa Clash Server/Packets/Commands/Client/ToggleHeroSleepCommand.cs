using UCS.Helpers.Binary;
using UCS.Helpers.Binary;

namespace UCS.Packets.Commands.Client
{
    // Packet 529
    internal class ToggleHeroSleepCommand : Command
    {
        public ToggleHeroSleepCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.FlagSleep = this.Reader.ReadByte();
            this.Tick = this.Reader.ReadUInt32();
        }

        public int BuildingId;
        public byte FlagSleep;
        public uint Tick;
    }
}