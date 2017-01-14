using System;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10117
    internal class ReportPlayerMessage : Message
    {
        public ReportPlayerMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public int ReportedPlayerID { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                br.ReadInt32();
                br.ReadInt32();
                ReportedPlayerID = br.ReadInt32();
                br.ReadInt32();               
            }
        }

        public override void Process(Level level)
        {
            /*var reportedPlayer = ResourcesManager.GetPlayer(ReportedPlayerID);

            new AvatarChatBanMessage(reportedPlayer.GetClient());*/
        }
    }
}
