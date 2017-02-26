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
            Count    = br.ReadInt32WithEndian();
            br.ReadUInt32WithEndian();
            Tick     = br.ReadInt32WithEndian();
        }

        public int Count { get; set; }
        public int UnitType { get; set; }
        public int Tick { get; set; }

        public override void Execute(Level level)
        {
            ClientAvatar _Player = level.GetHomeOwnerAvatar();

            if (UnitType.ToString().StartsWith("400"))
            {
                CombatItemData _TroopData = (CombatItemData)CSVManager.DataTables.GetDataById(UnitType);
                List<DataSlot> _PlayerUnits = level.GetPlayerAvatar().GetUnits();
                ResourceData _TrainingResource = _TroopData.GetTrainingResource();

                if (_TroopData != null)
                {
                    DataSlot _DataSlot = _PlayerUnits.Find(t => t.Data.GetGlobalID() == _TroopData.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value + this.Count;
                    }
                    else
                    {
                        DataSlot ds = new DataSlot(_TroopData, this.Count);
                        _PlayerUnits.Add(ds);
                    }

                    _Player.SetResourceCount(_TrainingResource, _Player.GetResourceCount(_TrainingResource) - _TroopData.GetTrainingCost(_Player.GetUnitUpgradeLevel(_TroopData)));
                }
            }
            else if (UnitType.ToString().StartsWith("260"))
            {
                SpellData _SpellData = (SpellData)CSVManager.DataTables.GetDataById(UnitType);
                List<DataSlot> _PlayerSpells = level.GetPlayerAvatar().GetSpells();
                ResourceData _CastResource = _SpellData.GetTrainingResource();

                if (_SpellData != null)
                {
                    DataSlot _DataSlot = _PlayerSpells.Find(t => t.Data.GetGlobalID() == _SpellData.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value + this.Count;
                    }
                    else
                    {
                        DataSlot ds = new DataSlot(_SpellData, this.Count);
                        _PlayerSpells.Add(ds);
                    }

                    _Player.SetResourceCount(_CastResource, _Player.GetResourceCount(_CastResource) - _SpellData.GetTrainingCost(_Player.GetUnitUpgradeLevel(_SpellData)));
                }
            }
        }
    }
}

