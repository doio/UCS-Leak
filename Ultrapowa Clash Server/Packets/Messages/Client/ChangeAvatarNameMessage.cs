using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10212
    internal class ChangeAvatarNameMessage : Message
    {
        public ChangeAvatarNameMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        string PlayerName { get; set; }  

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                PlayerName = br.ReadString();
            }
        }

        public override void Process(Level level)
        {
            if (string.IsNullOrEmpty(PlayerName) || PlayerName.Length > 15)
            {
                ResourcesManager.DisconnectClient(Client);
            }
            else
            {
                level.GetPlayerAvatar().SetName(PlayerName);
                AvatarNameChangeOkMessage p = new AvatarNameChangeOkMessage(Client);
                p.SetAvatarName(level.GetPlayerAvatar().GetAvatarName());
                p.Send();
            }
            //new RequestConfirmChangeNameMessage(Client, PlayerName);
        }
    }
}
