using System;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class GetIdGameopCommand : GameOpCommand
    {
        readonly string[] m_vArgs;

        public GetIdGameopCommand(string[] args)
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
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(avatar.UserID);
                    mail.SetSenderAvatarId(avatar.UserID);
                    mail.SetSenderName(avatar.Username);
                    mail.SetIsNew(0);
                    mail.SetAllianceId(1);
                    mail.SetAllianceBadgeData(1526735450);
                    mail.SetAllianceName("UCS Information");
                    mail.SetMessage("Your Player ID: " + level.Avatar.UserID);
                    mail.SetSenderLevel(avatar.Level);
                    mail.SetSenderLeagueId(avatar.League);

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
