using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 511
    internal class RequestAllianceUnitsCommand : Command
    {
        public RequestAllianceUnitsCommand(PacketReader br)
        {
            br.ReadUInt32WithEndian();
            FlagHasRequestMessage = br.ReadByte();
            Message = br.ReadString();
            Message2 = br.ReadString();
        }

        public override async void Execute(Level level)
        {           
            try
            {
                ClientAvatar player = level.GetPlayerAvatar();
                TroopRequestStreamEntry cm = new TroopRequestStreamEntry();
                Alliance all = await ObjectManager.GetAlliance(player.GetAllianceId());

                cm.SetId(all.GetChatMessages().Count + 1);
                cm.SetSenderId(player.GetId());
                cm.SetHomeId(player.GetId());
                cm.SetSenderLeagueId(player.GetLeagueId());
                cm.SetSenderName(player.GetAvatarName());
                cm.SetSenderRole(await player.GetAllianceRole());
                cm.SetMessage(Message);
                cm.SetMaxTroop(player.GetAllianceCastleTotalCapacity());

                all.AddChatMessage(cm);

                StreamEntry s = all.GetChatMessages().Find(c => c.GetSenderId() == level.GetPlayerAvatar().GetId() && c.GetStreamEntryType() == 1);
                if (s == null)
                {
                    all.GetChatMessages().RemoveAll(t => t == s);
                }
                all.AddChatMessage(cm);

                foreach (AllianceMemberEntry op in all.GetAllianceMembers())
                {
                    Level aplayer = await ResourcesManager.GetPlayer(op.GetAvatarId());
                    if (aplayer.GetClient() != null)
                    {
                        if (s != null)
                        {
                            PacketProcessor.Send(new AllianceStreamEntryRemovedMessage(aplayer.GetClient(), s.GetId()));
                        }
                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(aplayer.GetClient());
                        p.SetStreamEntry(cm);
                        PacketProcessor.Send(p);
                    }
                }
            }
            catch (Exception)
            {
                ResourcesManager.DropClient(level.GetClient().GetSocketHandle());
            }
        }

        public byte FlagHasRequestMessage { get; set; }
        public string Message { get; set; }
        public int MessageLength { get; set; }
        public string Message2 { get; set; }
    }
}
