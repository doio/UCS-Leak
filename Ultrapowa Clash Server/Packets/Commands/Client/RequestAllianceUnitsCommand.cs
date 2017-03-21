using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 511
    internal class RequestAllianceUnitsCommand : Command
    {
        public RequestAllianceUnitsCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            this.FlagHasRequestMessage = this.Reader.ReadByte();
            this.Message = this.Reader.ReadString();
            this.Message2 = this.Reader.ReadString();
        }

        internal override async void Process()
        {
            try
            {
                ClientAvatar player = this.Device.Player.Avatar;
                TroopRequestStreamEntry cm = new TroopRequestStreamEntry();
                Alliance all = await ObjectManager.GetAlliance(player.AllianceID);

                cm.SetId(all.GetChatMessages().Count + 1);
                cm.SetSenderId(player.UserID);
                cm.SetHomeId(player.UserID);
                cm.SetSenderLeagueId(player.League);
                cm.SetSenderName(player.Username);
                cm.SetSenderRole(await player.GetAllianceRole());
                cm.SetMessage(Message);
                cm.SetMaxTroop(player.Castle_Total);

                all.AddChatMessage((TroopRequestStreamEntry)cm);

                StreamEntry s = all.GetChatMessages().Find(c => c.GetSenderId() == this.Device.Player.Avatar.UserID && c.GetStreamEntryType() == 1);
                if (s == null)
                {
                    all.GetChatMessages().RemoveAll(t => t == s);
                }

                foreach (AllianceMemberEntry op in all.GetAllianceMembers())
                {
                    Level aplayer = await ResourcesManager.GetPlayer(op.AvatarID);
                    if (aplayer.Client != null)
                    {
                        if (s != null)
                        {
                            new AllianceStreamEntryRemovedMessage(aplayer.Client, s.GetId()).Send();
                        }
                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(aplayer.Client);
                        p.SetStreamEntry(cm);
                        p.Send();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public byte FlagHasRequestMessage;
        public string Message;
        public int MessageLength;
        public string Message2;
    }
}
