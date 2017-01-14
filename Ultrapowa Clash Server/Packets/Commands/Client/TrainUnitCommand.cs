using System;
using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 508
    internal class TrainUnitCommand : Command
    {
        public TrainUnitCommand(PacketReader br)
        {
            br.ReadInt32WithEndian();
            br.ReadUInt32WithEndian();
            UnitType = br.ReadInt32WithEndian();
            Count = br.ReadInt32WithEndian();
            br.ReadUInt32WithEndian();
            Tick = br.ReadInt32WithEndian();
        }

        public int Count { get; set; }
        public int UnitType { get; set; }
        public int Tick { get; set; }

        public override void Execute(Level level)
        {
            /*CombatItemData troopData = (CombatItemData)CSVManager.DataTables.GetDataById(UnitType);
            List<DataSlot> PlayerUnits = level.GetPlayerAvatar().GetUnits();

            if(troopData != null)
            {
                
            }      */
        }
    }
}
