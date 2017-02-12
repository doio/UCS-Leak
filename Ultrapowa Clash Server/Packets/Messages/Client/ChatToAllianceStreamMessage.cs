using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;
using System.Threading.Tasks;

namespace UCS.Packets.Messages.Client
{
    // Packet 14315
    internal class ChatToAllianceStreamMessage : Message
    {
        string m_vChatMessage;

        public ChatToAllianceStreamMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                m_vChatMessage = br.ReadScString();
            }
        }

        public override async void Process(Level level)
        {
            try {
                if (m_vChatMessage.Length > 0)
                {
                    if (m_vChatMessage.Length < 101)
                    {
                        if (m_vChatMessage[0] == '/')
                        {
                            Object obj = GameOpCommandFactory.Parse(m_vChatMessage);
                            if (obj != null)
                            {
                                string player = "";
                                if (level != null)
                                    player += " (" + level.GetPlayerAvatar().GetId() + ", " +
                                              level.GetPlayerAvatar().GetAvatarName() + ")";
                                ((GameOpCommand)obj).Execute(level);
                            }
                        }
                        else
                        {
                            ClientAvatar avatar = level.GetPlayerAvatar();
                            long allianceId = avatar.GetAllianceId();
                            if (allianceId > 0)
                            {
                                ChatStreamEntry cm = new ChatStreamEntry();
                                cm.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                                cm.SetSender(avatar);
                                cm.SetMessage(m_vChatMessage);

                                Alliance alliance = await ObjectManager.GetAlliance(allianceId);
                                if (alliance != null)
                                {
                                    alliance.AddChatMessage(cm);

                                    foreach (var op in alliance.GetAllianceMembers())
                                    {
                                        Level player = await ResourcesManager.GetPlayer(op.GetAvatarId());
                                        if (player.GetClient() != null)
                                        {
                                            AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(player.GetClient());
                                            p.SetStreamEntry(cm);
                                            PacketProcessor.Send(p);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception) { }
        }
    }
}