using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Checker;
using UCS.Core.Network;
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
                if (Message[0] == '/')
                {
                    object obj = GameOpCommandFactory.Parse(Message);
                    if (obj != null)
                    {
                        string player = "";
                        if (level != null)
                            player += " (" + level.GetPlayerAvatar().GetId() + ", " +
                                      level.GetPlayerAvatar().GetAvatarName() + ")";
                        ((GameOpCommand) obj).Execute(level);
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
                            p.Send();
                        }
                    }
                    else
                    {
                        foreach (Level onlinePlayer in ResourcesManager.GetOnlinePlayers())
                        {
                            GlobalChatLineMessage p = new GlobalChatLineMessage(onlinePlayer.GetClient());
                            p.SetPlayerName(senderName);
                            p.SetChatMessage(Message);
                            p.SetPlayerId(senderId);
                            p.SetLeagueId(level.GetPlayerAvatar().GetLeagueId());
                            p.SetAlliance(ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId()));
                            p.Send();
                            Logger.Write("Chat Message: '" + Message + "' from '" + senderName + "':'" + senderId + "'");
                        }
                    }
                }
            }
        }
    }
}
