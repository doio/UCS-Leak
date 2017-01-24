using System;
using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 522
    internal class BuyShieldCommand : Command
    {
        public BuyShieldCommand(PacketReader br)
        {
            ShieldId = br.ReadInt32WithEndian(); 
            Tick = br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            ClientAvatar player = level.GetPlayerAvatar();
            /*if (ShieldId == 20000000)
            {
                player.SetProtectionTime(Convert.ToInt32(TimeSpan.FromHours(((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).TimeH).TotalSeconds));
                player.UseDiamonds(((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).Diamonds);
            }
            else
            {*/
                player.SetShieldTime(player.GetShieldTime + Convert.ToInt32(TimeSpan.FromHours(((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).TimeH).TotalSeconds));
                player.UseDiamonds(((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).Diamonds);
            //}
        }

        public int ShieldId { get; set; }
        public int Tick { get; set; }
    }
}
