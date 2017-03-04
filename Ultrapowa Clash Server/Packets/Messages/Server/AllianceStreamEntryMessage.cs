using UCS.Logic.StreamEntry;

namespace UCS.Packets.Messages.Server
{
    // Packet 24312
    internal class AllianceStreamEntryMessage : Message
    {
        StreamEntry m_vStreamEntry;

        public AllianceStreamEntryMessage(Device client) : base(client)
        {
            this.Identifier = 24312;
        }

        internal override void Encode()
        {
            this.Data.AddRange(m_vStreamEntry.Encode());
        }

        public void SetStreamEntry(StreamEntry entry)
        {
            m_vStreamEntry = entry;
        }
    }
}