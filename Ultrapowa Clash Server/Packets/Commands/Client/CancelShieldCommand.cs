using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 534
    internal class CancelShieldCommand : Command
    {
        public CancelShieldCommand(PacketReader br)
        {
        }

        public override void Execute(Level level)
        {
            /*var Avatar = level.GetPlayerAvatar();
            var home = new ClientHome(Avatar.GetId());

            var shield = home.GetShieldTime();

            if(shield >= 1)
            {
                home.SetShieldTime(0);
            }     */
        }
    }      
}
