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

        public override void Execute(Level level)
        {
            /*
            var player = level.GetPlayerAvatar();
            var cm = new TroopRequestStreamEntry();
            var all = ObjectManager.GetAlliance(player.GetAllianceId());

            cm.SetId(all.GetChatMessages().Count + 1);
            cm.SetSenderId(player.GetId());
            cm.SetHomeId(player.GetId());
            cm.SetSenderLeagueId(player.GetLeagueId());
            cm.SetSenderName(player.GetAvatarName());
            cm.SetSenderRole(player.GetAllianceRole());
            cm.SetMessage(Message);
            cm.SetMaxTroop(player.GetAllianceCastleTotalCapacity());  
          
            all.AddChatMessage(cm);

            foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == player.GetAllianceId())
                {
                    var p = new AllianceStreamEntryMessage(onlinePlayer.GetClient());
                    p.SetStreamEntry(cm);
                    p.Send();
                }
            */
        }

        public byte FlagHasRequestMessage { get; set; }
        public string Message { get; set; }
        public int MessageLength { get; set; }
        public string Message2 { get; set; }
    }
}
