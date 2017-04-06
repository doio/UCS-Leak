using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class ServerStatusGameOpCommand   : GameOpCommand
    {
        readonly string[] m_vArgs;

        public ServerStatusGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.Avatar.AccountPrivileges >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 1)
                {
                    var avatar = level.Avatar;
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SenderId = avatar.UserId;
                    mail.SetSenderAvatarId(avatar.UserId);
                    mail.SetSenderName(avatar.AvatarName);
                    mail.SetIsNew(2);
                    mail.AllianceId = 0;
                    mail.AllianceBadgeData = 1526735450;
                    mail.AllianceName = "UCS Server Information";
					mail.Message = @"Online Players: " + ResourcesManager.m_vOnlinePlayers.Count +
						"\nIn Memory Players: " + ResourcesManager.m_vInMemoryLevels.Count +
						"\nConnected Players: " + ResourcesManager.GetConnectedClients().Count +
						"\nServer Ram: " + Performances.GetUsedMemory() + "% / " + Performances.GetTotalMemory() + "MB";

                    mail.SetSenderLevel(avatar.m_vAvatarLevel);
                    mail.SetSenderLeagueId(avatar.m_vLeagueId);

                    var p = new AvatarStreamEntryMessage(level.Client);
                    p.SetAvatarStreamEntry(mail);
                    Processor.Send(p);
                }
            }
            else
            {
                SendCommandFailedMessage(level.Client);
            }
        }
    }
}
