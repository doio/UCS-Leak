using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.Enums;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class MinRessourcesCommand : GameOpCommand
    {
        public MinRessourcesCommand(string[] Args)
        {
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.Avatar.AccountPrivileges >= GetRequiredAccountPrivileges())
            {
                ClientAvatar p = level.Avatar;
                p.Resources.Set(Resource.Gold, 1000);
                p.Resources.Set(Resource.Elixir, 1000);
                p.Resources.Set(Resource.DarkElixir, 1000);
                p.SetDiamonds(200);
                new OwnHomeDataMessage(level.Client, level).Send();
            }
            else
                SendCommandFailedMessage(level.Client);
        }
    }
}
