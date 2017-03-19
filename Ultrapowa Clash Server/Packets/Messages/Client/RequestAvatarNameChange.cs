using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14600
    internal class RequestUsernameChange : Message
    {
        public RequestUsernameChange(Device device, Reader reader) : base(device, reader)
        {
        }

        public string PlayerName { get; set; }

        public byte Unknown1 { get; set; }

        internal override void Decode()
        {
            this.PlayerName = this.Reader.ReadString();
        }

        internal override async void Process()
        {
            try
            {
                long id = this.Device.Player.Avatar.UserID;
                Level l = await ResourcesManager.GetPlayer(id);
                if (l != null)
                {
                    if (PlayerName.Length > 15)
                    {
                        ResourcesManager.DisconnectClient(Device);
                    }
                    else
                    {
                        l.Avatar.SetName(PlayerName);
                        UsernameChangeOkMessage p = new UsernameChangeOkMessage(l.Client) {Username = PlayerName};
                        p.Send();
                    }
                }
            } catch (Exception) { }
        }
    }
}