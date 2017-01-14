using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 558
    internal class AddQuicKTrainingTroopCommand : Command
    {
        public int Database;
        public int Tick;
        public int TroopType;
        public List<UnitToAdd> UnitsToAdd { get; set; }
        public AddQuicKTrainingTroopCommand(PacketReader br)
        {
            Database = br.ReadInt32();
            TroopType = br.ReadInt32();
            UnitsToAdd = new List<UnitToAdd>();
            for (int i = 0; i < TroopType; i++)
                UnitsToAdd.Add(new UnitToAdd { Data = (CharacterData)br.ReadDataReference(), Count = br.ReadInt32() });
            Tick = br.ReadInt32();
        }

        public override void Execute(Level level)
        {
            var defaultdatbase = level.GetPlayerAvatar().QuickTrain1;
            if (Database == 1)
                defaultdatbase.Clear();
            else if (Database == 2)
            {
                defaultdatbase = level.GetPlayerAvatar().QuickTrain2;
                defaultdatbase.Clear();
            }
            else if (Database == 3)
            {
                defaultdatbase = level.GetPlayerAvatar().QuickTrain3;
                defaultdatbase.Clear();
            }
            else
                throw new NullReferenceException("Unknown Database Detected");

            foreach (var i in UnitsToAdd)
                {
                    DataSlot ds = new DataSlot(i.Data, i.Count);
                    defaultdatbase.Add(ds);
                }
        }
        internal class UnitToAdd
        {
            public int Count { get; set; }
            public CharacterData Data { get; set; }
        }
    }
}
