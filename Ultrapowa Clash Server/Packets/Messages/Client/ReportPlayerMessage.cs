using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
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

        public long ReportedPlayerID { get; set; }

        public int Tick { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                br.ReadInt32();
                ReportedPlayerID = br.ReadInt64();
                br.ReadInt32();               
            }
        }

        public override async void Process(Level level)
        {
            try
            {
                Level ReportedPlayer = await ResourcesManager.GetPlayer(ReportedPlayerID);
                ReportedPlayer.GetPlayerAvatar().ReportedTimes++;
                if (ReportedPlayer.GetPlayerAvatar().ReportedTimes >= 3)
                {
                    AvatarChatBanMessage _AvatarChatBanMessage = new AvatarChatBanMessage(ReportedPlayer.GetClient());
                    //_AvatarChatBanMessage.SetBanPeriod(86400); // A Day
                    _AvatarChatBanMessage.SetBanPeriod(1800); // 30 Minutes
                    PacketProcessor.Send(_AvatarChatBanMessage);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
