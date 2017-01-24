using System;
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
            Tick = br.ReadInt32();
        }

        public int Tick { get; set; }

        public override void Execute(Level level)
        {
            ClientAvatar player = level.GetPlayerAvatar();

            if (player.GetShieldTime != null)
            {
                player.SetShieldTime(0);
                //player.SetProtectionTime(1800);
            }
            /*else 
            {
                player.SetShieldTime(0);
                player.SetProtectionTime(0);
            }*/
        }
    }      
}
