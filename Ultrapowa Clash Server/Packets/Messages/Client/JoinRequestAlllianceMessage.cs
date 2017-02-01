using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14317
    internal class JoinRequestAllianceMessage : Message
    {
        public JoinRequestAllianceMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public static string Message { get; set; }

        public static long ID { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                ID = br.ReadInt64();
                Message = br.ReadScString();
            }
        }


        public override void Process(Level level)
        {
            ClientAvatar player = level.GetPlayerAvatar();
            Alliance all = ObjectManager.GetAlliance(ID);

            InvitationStreamEntry cm = new InvitationStreamEntry();
            cm.SetId(all.GetChatMessages().Count + 1);
            cm.SetSenderId(player.GetId());
            cm.SetHomeId(player.GetId());
            cm.SetSenderLeagueId(player.GetLeagueId());
            cm.SetSenderName(player.GetAvatarName());
            cm.SetSenderRole(player.GetAllianceRole());
            cm.SetMessage(Message);
            cm.SetState(1);
            all.AddChatMessage(cm);

            foreach (AllianceMemberEntry op in all.GetAllianceMembers())
            {
                Level playera = ResourcesManager.GetPlayer(op.GetAvatarId());
                if (playera.GetClient() != null)
                {
                    AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(playera.GetClient());
                    p.SetStreamEntry(cm);
                    PacketManager.Send(p);
                }
            }
        }
    }
}