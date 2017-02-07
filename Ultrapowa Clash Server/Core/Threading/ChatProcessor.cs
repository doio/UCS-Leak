using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;
using UCS.Helpers;

namespace UCS.Core.Threading
{
    class ChatProcessor
    {
        public static List<GlobalChatLineMessage> Messages = null;

        public ChatProcessor()
        {
            new Thread((ThreadStart)(() =>
            {
                Messages = new List<GlobalChatLineMessage>();

                while (true)
                {
                    if (Messages.Any())
                    {
                        try
                        {
                            foreach (GlobalChatLineMessage cl in Messages)
                            {
                                PacketProcessor.Send(cl);
                                Messages.Remove(cl);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    Thread.Sleep(50);
                }
            })).Start();
        }

        public static void AddMessage(GlobalChatLineMessage gch)
        {
            try
            {
                Messages.Add(gch);
            }
            catch (Exception)
            {
            }
        }
    }
}
