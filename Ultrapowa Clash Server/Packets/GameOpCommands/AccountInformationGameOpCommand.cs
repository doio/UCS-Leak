using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class AccountInformationGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;
        private string Message;
        public AccountInformationGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(5);
        }
        public override async void Execute(Level level)
        {
            if (level.Avatar.AccountPrivileges >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 2)
                {
                    try
                    {
                        long id = Convert.ToInt64(m_vArgs[1]);
                        Level l = await ResourcesManager.GetPlayer(id);
                        if (l != null)
                        {
                            ClientAvatar acc = l.Avatar;
                            Message = "Player Info : \n\n" + "ID = " + id + "\nName = " + acc.Username +
                                      "\nCreation Date : " + acc.Created + "\nRegion : " + acc.Region +
                                      "\nIP Address : " + l.Avatar.IPAddress;
                            if (acc.AllianceID != 0)
                            {
                                Alliance a = await ObjectManager.GetAlliance(acc.AllianceID);
                                Message = Message + "\nClan Name : " + a.GetAllianceName();
                                switch (await acc.GetAllianceRole())
                                {
                                    case 1:
                                        Message = Message + "\nClan Role : Member";
                                        break;

                                    case 2:
                                        Message = Message + "\nClan Role : Leader";
                                        break;

                                    case 3:
                                        Message = Message + "\nClan Role : Elder";
                                        break;

                                    case 4:
                                        Message = Message + "\nClan Role : Co-Leader";
                                        break;

                                    default:
                                        Message = Message + "\nClan Role : Unknown";
                                        break;
                                }
                            }
                            Message = Message + "\nLevel : " + acc.Level + "\nTrophy : " + acc.GetTrophies() +
                                      "\nTown Hall Level : " + (acc.TownHall_Level + 1) +
                                      "\nAlliance Castle Level : " + (acc.Castle_Level+ 1);

                            var avatar = level.Avatar;
                            AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                            mail.SetSenderId(avatar.UserID);
                            mail.SetSenderAvatarId(avatar.UserID);
                            mail.SetSenderName(avatar.Username);
                            mail.SetIsNew(2);
                            mail.SetAllianceId(0);
                            mail.SetAllianceBadgeData(1526735450);
                            mail.SetAllianceName("UCS Server Information");
                            mail.SetMessage(Message);
                            mail.SetSenderLevel(avatar.Level);
                            mail.SetSenderLeagueId(avatar.League);

                            AvatarStreamEntryMessage p = new AvatarStreamEntryMessage(level.Client);
                            p.SetAvatarStreamEntry(mail);
                            Processor.Send(p);
                        }
                    }
                    catch (Exception)
                    {
                        GlobalChatLineMessage c = new GlobalChatLineMessage(level.Client)
                        {
                            Message = "Command Failed, Wrong Format Or User Doesn't Exist (/accinfo id).",
                            HomeId = level.Avatar.UserID,
                            CurrentHomeId = level.Avatar.UserID,
                            LeagueId = 22,
                            PlayerName = "Ultrapowa Clash Server"
                        };

                        Processor.Send(c);
                        return;
                    }
                }
                else
                {
                    GlobalChatLineMessage b = new GlobalChatLineMessage(level.Client)
                    {
                        Message = "Command Failed, Wrong Format (/accinfo id).",
                        HomeId = level.Avatar.UserID,
                        CurrentHomeId = level.Avatar.UserID,
                        LeagueId = 22,
                        PlayerName = "Ultrapowa Clash Server"
                    };
                    Processor.Send(b);
                }
            }
        }
    }
}


