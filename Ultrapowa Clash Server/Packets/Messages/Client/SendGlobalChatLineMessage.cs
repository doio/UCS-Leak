using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Checker;
using UCS.Core.Network;
using UCS.Core.Threading;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14715
    internal class SendGlobalChatLineMessage : Message
    {
        public SendGlobalChatLineMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public string Message { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                Message = br.ReadString();
            }
        }

        public override void Process(Level level)
        {
            if (Message.Length > 0)
            {
                if (Message.Length < 101)
                {
                    if (Message[0] == '/')
                    {
                        object obj = GameOpCommandFactory.Parse(Message);
                        if (obj != null)
                        {
                            string player = "";
                            if (level != null)
                                player += " (" + level.GetPlayerAvatar().GetId() + ", " +
                                          level.GetPlayerAvatar().GetAvatarName() + ")";
                            ((GameOpCommand)obj).Execute(level);
                        }
                    }
                    else
                    {
                        long senderId = level.GetPlayerAvatar().GetId();
                        string senderName = level.GetPlayerAvatar().GetAvatarName();

                        bool badword = DirectoryChecker.badwords.Any(s => Message.Contains(s));

                        if (badword)
                        {
                            foreach (Level pl in ResourcesManager.GetOnlinePlayers())
                            {
                                /*if (pl.GetPlayerAvatar().GetUserRegion() == level.GetPlayerAvatar().GetUserRegion())
                                {*/
                                    GlobalChatLineMessage p = new GlobalChatLineMessage(pl.GetClient());
                                    string NewMessage = "";

                                    for (int i = 0; i < Message.Length; i++)
                                    {
                                        NewMessage += "*";
                                    }

                                    p.SetPlayerName(senderName);
                                    p.SetChatMessage(NewMessage);
                                    p.SetPlayerId(senderId);
                                    p.SetLeagueId(level.GetPlayerAvatar().GetLeagueId());
                                    p.SetAlliance(ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId()));

                                    ChatProcessor.AddMessage(p);
                                    //PacketManager.Send(p);
                                //}
                            }
                        }
                        else
                        {
                            foreach (Level onlinePlayer in ResourcesManager.GetOnlinePlayers())
                            {
                                /*if (onlinePlayer.GetPlayerAvatar().GetUserRegion() == level.GetPlayerAvatar().GetUserRegion())
                                {*/
                                    GlobalChatLineMessage p = new GlobalChatLineMessage(onlinePlayer.GetClient());
                                    p.SetPlayerName(senderName);
                                    p.SetChatMessage(Message);
                                    p.SetPlayerId(senderId);
                                    p.SetLeagueId(level.GetPlayerAvatar().GetLeagueId());
                                    p.SetAlliance(ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId()));
                                    //PacketManager.Send(p);
                                    ChatProcessor.AddMessage(p);
                                    Logger.Write("Chat Message: '" + Message + "' from '" + senderName + "':'" + senderId + "'");
                                //}
                            }
                        }
                    }
                }
            }
        }
    }
}
