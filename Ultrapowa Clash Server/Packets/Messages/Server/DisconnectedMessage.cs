using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 25892
    internal class DisconnectedMessage : Message
    {
        public static int PacketID = 25892;

		public DisconnectedMessage(PacketReader br, Packets.Client client) : base(client)
		{
		}

		public override void Encode()
		{
		}

		public override void Process(Level level)
		{
			//ResourcesManager.DropClient(level.GetClient().GetSocketHandle());
		}
	}
}
