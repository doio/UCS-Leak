using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
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

        public ChatToAllianceStreamMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
            this.m_vChatMessage = this.Reader.ReadString();
        }

        internal override async void Process()
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
                                if (this.Device.Player != null)
                                    player += " (" + this.Device.Player.Avatar.GetId() + ", " +
                                              this.Device.Player.Avatar.AvatarName + ")";
                                ((GameOpCommand)obj).Execute(this.Device.Player);
                            }
                        }
                        else
                        {
                            ClientAvatar avatar = this.Device.Player.Avatar;
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
                                        if (player.Client != null)
                                        {
                                            AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(player.Client);
                                            p.SetStreamEntry(cm);
                                            p.Send();
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