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
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            /*Console.WriteLine("Shield ID: " + ShieldId);
            Console.WriteLine("Shiled TID: " + ((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).TID);
            Console.WriteLine("Cooldown: " + ((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).CooldownS);
            Console.WriteLine("Duration: " + ((ShieldData)CSVManager.DataTables.GetDataById(ShieldId)).TimeH);*/
            //Console.WriteLine(Unknown1);
        }

        public int ShieldId { get; set; }
        public uint Unknown1 { get; set; }
    }
}
