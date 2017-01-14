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
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 1)
                {
                    var avatar = level.GetPlayerAvatar();
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(avatar.GetId());
                    mail.SetSenderAvatarId(avatar.GetId());
                    mail.SetSenderName(avatar.GetAvatarName());
                    mail.SetIsNew(2);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(1526735450);
                    mail.SetAllianceName("UCS Server Information");
					mail.SetMessage(@"Online Players: " + ResourcesManager.GetOnlinePlayers().Count +
						"\nIn Memory Players: " + ResourcesManager.GetInMemoryLevels().Count +
						"\nConnected Players: " + ResourcesManager.GetConnectedClients().Count +
						"\nServer Ram: " + Performances.GetUsedMemory() + "% / " + Performances.GetTotalMemory() + "MB"
						);

                    mail.SetSenderLevel(avatar.GetAvatarLevel());
                    mail.SetSenderLeagueId(avatar.GetLeagueId());

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    p.Send();
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}
