using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Core.Threading;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class HelpGameOpCommand: GameOpCommand
    {
        readonly string[] m_vArgs;

        public HelpGameOpCommand(string[] args)
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
                    mail.AllianceName = "UCS Server Commands Help";
                    mail.Message = @"/help" +
                        "\n/ban" +
                        "\n/kick" +
                        "\n/rename" +
                        "\n/setprivileges" +
                        "\n/shutdown" +
                        "\n/unban" +
                        "\n/visit" +
                        "\n/sysmsg" +
                        "\n/id" +
                        "\n/max" +
                        "\n/saveacc" +
                        "\n/saveall" +
                        "\n/becomeleader" +
                        "\n/status";
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
