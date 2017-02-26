using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 570
    internal class TogglePlayerWarStateCommand : Command
    {
        public TogglePlayerWarStateCommand(PacketReader br)
        {
            br.ReadInt32();
            br.ReadInt32();
        }

        public override async void Execute(Level level)
        {
            try
            {
                Alliance a = await ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
                if (a != null)
                {
                    AllianceMemberEntry _AllianceMemberEntry = a.GetAllianceMember(level.GetPlayerAvatar().GetId());
                    _AllianceMemberEntry.ToggleStatus();
                    PlayerWarStatusMessage _PlayerWarStatusMessage = new PlayerWarStatusMessage(level.GetClient());
                    _PlayerWarStatusMessage.SetStatus(_AllianceMemberEntry.GetStatus());
                    PacketProcessor.Send(_PlayerWarStatusMessage);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}