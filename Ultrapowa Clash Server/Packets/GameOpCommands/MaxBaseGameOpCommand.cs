using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    class MaxBaseGameOpCommand : GameOpCommand
    {
        public MaxBaseGameOpCommand(string[] Args)
        {
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                string Home;

                using (StreamReader sr = new StreamReader(@"Gamefiles/level/PVP/Base55.json"))
                {
                    Home = sr.ReadToEnd();
                    ResourcesManager.SetGameObject(level, Home);
                    new OutOfSyncMessage(level.GetClient()).Send();
                }
            }
            else
                SendCommandFailedMessage(level.GetClient());
        }
    }
}
