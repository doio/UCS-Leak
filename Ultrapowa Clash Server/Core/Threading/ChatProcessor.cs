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
        public static List<GlobalChatLineMessage> Messages = null;

        public ChatProcessor()
        {
            new Thread((ThreadStart)(() =>
            {
                Messages = new List<GlobalChatLineMessage>();

                while (true)
                {
                    try
                    {
                        foreach (GlobalChatLineMessage cl in Messages)
                        {
                            PacketProcessor.Send(cl);
                            Messages.Remove(cl);
                        }
                    }
                    catch (Exception) {}
                    Thread.Sleep(10);
                }

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
