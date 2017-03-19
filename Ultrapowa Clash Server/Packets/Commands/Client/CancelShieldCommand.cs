using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 534
    internal class CancelShieldCommand : Command
    {
        public CancelShieldCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.Tick = this.Reader.ReadInt32();
        }

        public int Tick;

        internal override void Process()
        {
            ClientAvatar player = this.Device.Player.Avatar;

            //if (player.Shield > 0)
            //{
                player.Shield = 0;
                //player.SetProtectionTime(1800);
            //}
            /*else 
            {
                player.SetShieldTime(0);
                player.SetProtectionTime(0);
            }*/
        }
    }      
}
