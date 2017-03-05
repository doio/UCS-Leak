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
            ClientAvatar player = this.Device.Player.Avatar;
            TroopRequestStreamEntry cm = new TroopRequestStreamEntry();
            Alliance all = await ObjectManager.GetAlliance(player.GetAllianceId());

            cm.SetId(all.GetChatMessages().Count + 1);
            cm.SetSenderId(player.GetId());
            cm.SetHomeId(player.GetId());
            cm.SetSenderLeagueId(player.GetLeagueId());
            cm.SetSenderName(player.AvatarName);
            cm.SetSenderRole(await player.GetAllianceRole());
            cm.SetMessage(Message);
            cm.SetMaxTroop(player.GetAllianceCastleTotalCapacity());

            StreamEntry s = all.GetChatMessages().Find(c => c.GetSenderId() == this.Device.Player.Avatar.UserId && c.GetStreamEntryType() == 1);
            if (s == null)
            {
                all.GetChatMessages().RemoveAll(t => t == s);
            }

            foreach (AllianceMemberEntry op in all.GetAllianceMembers())
            {
                Level aplayer = await ResourcesManager.GetPlayer(op.GetAvatarId());
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

        public byte FlagHasRequestMessage;
        public string Message;
        public int MessageLength;
        public string Message2;
    }
}
