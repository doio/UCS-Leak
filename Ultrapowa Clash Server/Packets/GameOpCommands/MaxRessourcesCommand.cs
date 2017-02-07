using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
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
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                var p = level.GetPlayerAvatar();
                p.SetResourceCount(CSVManager.DataTables.GetResourceByName("Gold"), 999999999);
                p.SetResourceCount(CSVManager.DataTables.GetResourceByName("Elixir"), 999999999);
                p.SetResourceCount(CSVManager.DataTables.GetResourceByName("DarkElixir"), 999999999);
                p.SetDiamonds(999999999);
                PacketProcessor.Send(new OwnHomeDataMessage(level.GetClient(), level));
            }
            else
                SendCommandFailedMessage(level.GetClient());
        }
    }
}
