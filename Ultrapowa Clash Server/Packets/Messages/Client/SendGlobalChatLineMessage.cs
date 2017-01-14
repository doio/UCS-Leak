using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UCS.Core;
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
                    if (File.Exists(@"filter.ucs"))
                    {
                        long senderId = level.GetPlayerAvatar().GetId();
                        string senderName = level.GetPlayerAvatar().GetAvatarName();

                        List<string> badwords = new List<string>();
                        StreamReader r = new StreamReader(@"filter.ucs");
                        string line = "";
                        while ((line = r.ReadLine()) != null)
                        {
                            badwords.Add(line);
                        }
                        bool badword = badwords.Any(s => Message.Contains(s));

                        if (badword)
                        {
                            GlobalChatLineMessage p = new GlobalChatLineMessage(level.GetClient());
                            p.SetPlayerId(0);
                            p.SetPlayerName("Chat Filter System");
                            p.SetLeagueId(22);
                            p.SetChatMessage("DETECTED BAD WORD! PLEASE AVOID USING BAD WORDS!");
                            p.Send();
                            return;
                        }

                        foreach(Level onlinePlayer in ResourcesManager.GetOnlinePlayers())
                        {
                            GlobalChatLineMessage p = new GlobalChatLineMessage(onlinePlayer.GetClient());
                            if (onlinePlayer.GetAccountPrivileges() > 0)
                            {
                                p.SetPlayerName(senderName + " #" + senderId);
                            }
                            else
                            {
                                p.SetPlayerName(senderName);
                            }
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
