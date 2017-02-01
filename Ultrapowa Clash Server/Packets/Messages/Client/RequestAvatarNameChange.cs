using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14600
    internal class RequestAvatarNameChange : Message
    {
        public RequestAvatarNameChange(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public string PlayerName { get; set; }

        public byte Unknown1 { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                PlayerName = br.ReadString();
            }
        }

        public override void Process(Level level)
        {
            long id = level.GetPlayerAvatar().GetId();
            Level l = ResourcesManager.GetPlayer(id);
            if (l != null)
            {
                if (PlayerName.Length > 15)
                {
                    ResourcesManager.DisconnectClient(Client);
                }
                else
                {
                    l.GetPlayerAvatar().SetName(PlayerName);
                    AvatarNameChangeOkMessage p = new AvatarNameChangeOkMessage(l.GetClient());
                    p.SetAvatarName(PlayerName);
                    p.Send();
                }
            }
        }
    }
}