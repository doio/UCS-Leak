﻿using System;
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
    class GlobalChatThread
    {
        public static List<GlobalChatLineMessage> Messages;

        // THIS IS JUST FOR TESTING!

        public GlobalChatThread()
        {
            new Thread(() =>
            {
                Messages = new List<GlobalChatLineMessage>();

                loop:

                if (MessageIsWaiting())
                {
                    try
                    {
                        lock (Messages)
                        {
                            foreach (GlobalChatLineMessage cl in Messages)
                            {
                                cl.Send();
                                Messages.Remove(cl);
                            }
                        }
                    }
                    catch (Exception) { goto loop; }
                }

                goto loop;
            }).Start();
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
                lock (Messages)
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
            }
            catch (Exception) { return false; }
        }
    }
}
