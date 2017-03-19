using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10212
    internal class ChangeUsernameMessage : Message
    {
        public ChangeUsernameMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        string PlayerName { get; set; }  

        internal override void Decode()
        {
            this.PlayerName = this.Reader.ReadString();
        }

        internal override void Process()
        {
            if (string.IsNullOrEmpty(PlayerName) || PlayerName.Length > 15)
            {
                ResourcesManager.DisconnectClient(Device);
            }
            else
            {
                this.Device.Player.Avatar.SetName(PlayerName);
                UsernameChangeOkMessage p = new UsernameChangeOkMessage(this.Device)
                {
                    Username = this.Device.Player.Avatar.Username
                };
                p.Send();
            }
            //new RequestConfirmChangeNameMessage(Client, PlayerName);
        }
    }
}
