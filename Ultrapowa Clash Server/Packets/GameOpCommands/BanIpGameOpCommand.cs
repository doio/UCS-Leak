using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class BanIpGameOpCommand : GameOpCommand
    {
        readonly string[] m_vArgs;

        public BanIpGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(3);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
                if (m_vArgs.Length >= 1)
                    try
                    {
                        var id = Convert.ToInt64(m_vArgs[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if (l != null)
                            if (l.GetAccountPrivileges() < level.GetAccountPrivileges())
                            {
                                l.SetAccountStatus(99);
                                l.SetAccountPrivileges(0);
                                if (ResourcesManager.IsPlayerOnline(l))
                                {
                                    PacketProcessor.Send(new OutOfSyncMessage(l.GetClient()));
                                }
                            }
                            else
                            {
                            }
                        else
                        {
                        }
                    }
                    catch 
                    {
                    }
                else
                    SendCommandFailedMessage(level.GetClient());
        }
    }
}
