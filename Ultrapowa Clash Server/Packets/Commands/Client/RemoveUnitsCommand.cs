using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 550
    internal class RemoveUnitsCommand : Command
    {
        public RemoveUnitsCommand(PacketReader br)
        {
            br.ReadUInt32WithEndian();
            UnitTypesCount = br.ReadInt32WithEndian();

            UnitsToRemove = new List<UnitToRemove>();
            for (var i = 0; i < UnitTypesCount; i++)
            {
                int UnitType = br.ReadInt32WithEndian();
                int count = br.ReadInt32WithEndian();
                int level = br.ReadInt32WithEndian();
                UnitsToRemove.Add(new UnitToRemove { Data = UnitType, Count = count, Level = level });
            }

            br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            List<DataSlot> _PlayerUnits = level.GetPlayerAvatar().GetUnits();
            List<DataSlot> _PlayerSpells = level.GetPlayerAvatar().GetSpells();

            foreach (UnitToRemove _Unit in UnitsToRemove)
            {
                if (_Unit.Data.ToString().StartsWith("400"))
                {
                    CombatItemData _Troop = (CombatItemData)CSVManager.DataTables.GetDataById(_Unit.Data); ;
                    DataSlot _DataSlot = _PlayerUnits.Find(t => t.Data.GetGlobalID() == _Troop.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value - 1;
                    }
                }
                else if (_Unit.Data.ToString().StartsWith("260"))
                {
                    SpellData _Spell = (SpellData)CSVManager.DataTables.GetDataById(_Unit.Data); ;
                    DataSlot _DataSlot = _PlayerSpells.Find(t => t.Data.GetGlobalID() == _Spell.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value - 1;
                    }
                }
            }
        }

        public List<UnitToRemove> UnitsToRemove { get; set; }
        public int UnitTypesCount { get; set; }
    }

    internal class UnitToRemove
    {
        public int Data { get; set; }
        public int Count { get; set; }
        public int Level { get; set; }
    }
}
