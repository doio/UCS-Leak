using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14317
    internal class JoinRequestAllianceMessage : Message
    {
        public JoinRequestAllianceMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public string Message;

        public long ID;

        internal override void Decode()
        {
            this.ID      = this.Reader.ReadInt64();
            this.Message = this.Reader.ReadString();
        }


        internal override async void Process()
        {
            try
            {
                ClientAvatar player = this.Device.Player.Avatar;
                Alliance all = await ObjectManager.GetAlliance(ID);

                InvitationStreamEntry cm = new InvitationStreamEntry();
                cm.SetId(all.GetChatMessages().Count + 1);
                cm.SetSenderId(player.UserID);
                cm.SetHomeId(player.UserID);
                cm.SetSenderLeagueId(player.League);
                cm.SetSenderName(player.Username);
                cm.SetSenderRole(await player.GetAllianceRole());
                cm.SetMessage(Message);
                cm.SetState(1);
                all.AddChatMessage(cm);

                foreach (AllianceMemberEntry op in all.GetAllianceMembers())
                {
                    Level playera = await ResourcesManager.GetPlayer(op.AvatarID);
                    if (playera.Client != null)
                    {
                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(playera.Client);
                        p.SetStreamEntry(cm);
                        p.Send();
                    }
                }
            } catch (Exception) { }
        }
    }
}