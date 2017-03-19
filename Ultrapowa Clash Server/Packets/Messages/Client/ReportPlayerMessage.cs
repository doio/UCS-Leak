using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10117
    internal class ReportPlayerMessage : Message
    {
        public ReportPlayerMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public long ReportedPlayerID { get; set; }

        public int Tick { get; set; }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            this.ReportedPlayerID = this.Reader.ReadInt64();
            this.Reader.ReadInt32();
        }

        internal override async void Process()
        {
            try
            {
                if (this.Device.Player.Avatar.ReportedCount >= 3)
                {
                    ReportedPlayerMessage _ReportedPlayerMessage = new ReportedPlayerMessage(this.Device);
                    _ReportedPlayerMessage.SetID(6);
                    _ReportedPlayerMessage.Send();
                }
                else
                {
                    this.Device.Player.Avatar.ReportedCount++;

                    ReportedPlayerMessage _ReportedPlayerMessage = new ReportedPlayerMessage(this.Device);
                    _ReportedPlayerMessage.SetID(1);
                    _ReportedPlayerMessage.Send();

                    Level _ReportedPlayer = await ResourcesManager.GetPlayer(ReportedPlayerID);
                    _ReportedPlayer.Avatar.GotReported++;

                    if (_ReportedPlayer.Avatar.GotReported >= 3)
                    {
                        AvatarChatBanMessage _AvatarChatBanMessage = new AvatarChatBanMessage(_ReportedPlayer.Client);
                        _AvatarChatBanMessage.SetBanPeriod(1800);
                        _AvatarChatBanMessage.Send();
                    }
                }

                // 6 = Wait before send again;
                // 1 = Reported
                // 2 = Daily Limit reached;
            }
            catch (Exception)
            {
            }
        }
    }
}
