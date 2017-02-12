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
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 2)
                {
                    try
                    {
                        long id = Convert.ToInt64(m_vArgs[1]);
                        Level l = await ResourcesManager.GetPlayer(id);
                        if (l != null)
                        {
                            ClientAvatar acc = l.GetPlayerAvatar();
                            Message = "Player Info : \n\n" + "ID = " + id + "\nName = " + acc.GetAvatarName() + "\nCreation Date : " + acc.GetAccountCreationDate() + "\nRegion : " + acc.GetUserRegion() + "\nIP Address : " + l.GetIPAddress();
                            if (acc.GetAllianceId() != 0)
                            {
                                Alliance a = await ObjectManager.GetAlliance(acc.GetAllianceId());
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
                            Message = Message + "\nLevel : " + acc.GetAvatarLevel() + "\nTrophy : " + acc.GetScore() + "\nTown Hall Level : " + (acc.GetTownHallLevel() + 1)  + "\nAlliance Castle Level : " + (acc.GetAllianceCastleLevel() + 1);

                            var avatar = level.GetPlayerAvatar();
                            AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                            mail.SetSenderId(avatar.GetId());
                            mail.SetSenderAvatarId(avatar.GetId());
                            mail.SetSenderName(avatar.GetAvatarName());
                            mail.SetIsNew(2);
                            mail.SetAllianceId(0);
                            mail.SetAllianceBadgeData(1526735450);
                            mail.SetAllianceName("UCS Server Information");
                            mail.SetMessage(Message);
                            mail.SetSenderLevel(avatar.GetAvatarLevel());
                            mail.SetSenderLeagueId(avatar.GetLeagueId());

                            AvatarStreamEntryMessage p = new AvatarStreamEntryMessage(level.GetClient());
                            p.SetAvatarStreamEntry(mail);
                            PacketProcessor.Send(p);
                        }
                    }
                    catch (Exception)
                    {
                        GlobalChatLineMessage c = new GlobalChatLineMessage(level.GetClient());
                        c.SetChatMessage("Command Failed, Wrong Format Or User Doesn't Exist (/accinfo id).");
                        c.SetPlayerId(level.GetPlayerAvatar().GetId());
                        c.SetLeagueId(22);
                        c.SetPlayerName("Ultrapowa Clash Server");
                        PacketProcessor.Send(c);
                        return;
                    }
                }
                else
                {
                    GlobalChatLineMessage b = new GlobalChatLineMessage(level.GetClient());
                    b.SetChatMessage("Command Failed, Wrong Format (/accinfo id).");
                    b.SetPlayerId(level.GetPlayerAvatar().GetId());
                    b.SetLeagueId(22);
                    b.SetPlayerName("Ultrapowa Clash Server");
                    PacketProcessor.Send(b);
                }
            }
        }
    }
}


