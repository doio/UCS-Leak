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

namespace UCS.Core.Threading
{
    class ChatProcessor
    {
        public static List<GlobalChatLineMessage> Messages;

        public ChatProcessor()
        {
            new Thread((ThreadStart)(() =>
            {
                Messages = new List<GlobalChatLineMessage>();

                loop:

                if (MessageIsWaiting())
                {
                    try
                    {
                        foreach (GlobalChatLineMessage cl in Messages)
                        {
                            PacketProcessor.Send(cl);
                            Messages.Remove(cl);
                        }
                    }
                    catch (Exception) { goto loop; }
                }
                else if (!MessageIsWaiting())
                {
                    Thread.Sleep(100);
                    goto loop;
                }

                goto loop;
            })).Start();
        }

        public static void AddMessage(GlobalChatLineMessage ch)
        {
            try
            {
                Messages.Add(ch);
            }
            catch (Exception)
            {
            }
        }

        public static bool MessageIsWaiting()
        {
            try
            {
                if (!Messages.Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception) { return false; }
        }
    }
}
