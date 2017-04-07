using UCS.Helpers.Binary;

namespace UCS.Packets.Messages.Client
{
    // Packet 15001
    internal class AllianceWarAttackAvatarMessage : Message
    {
        public AllianceWarAttackAvatarMessage(Device device, Reader reader) : base(device, reader)
        {
            int PacketID = 15001;
        }
    }

}
