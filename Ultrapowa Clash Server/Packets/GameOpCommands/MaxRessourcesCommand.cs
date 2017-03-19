using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.Enums;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.GameOpCommands
{
    internal class MaxRessourcesCommand : GameOpCommand
    {
        public MaxRessourcesCommand(string[] Args)
        {
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.Avatar.AccountPrivileges >= GetRequiredAccountPrivileges())
            {
                var p = level.Avatar;
                p.Resources.Set(Resource.Gold, 999999999);
                p.Resources.Set(Resource.Elixir, 999999999);
                p.Resources.Set(Resource.DarkElixir, 999999999);
                p.SetDiamonds(999999999);
                Processor.Send(new OwnHomeDataMessage(level.Client, level));
            }
            else
                SendCommandFailedMessage(level.Client);
        }
    }
}
