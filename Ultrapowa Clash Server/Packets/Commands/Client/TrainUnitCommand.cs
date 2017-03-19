using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.JSONProperty;

namespace UCS.Packets.Commands.Client
{
    // Packet 508
    internal class TrainUnitCommand : Command
    {
        public TrainUnitCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {

        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            this.Reader.ReadUInt32();
            this.UnitType = this.Reader.ReadInt32();
            this.Count = this.Reader.ReadInt32();
            this.Reader.ReadUInt32();
            Tick = this.Reader.ReadInt32();
        }

        public int Count;
        public int UnitType;
        public int Tick;

        internal override void Process()
        {
            ClientAvatar _Player = this.Device.Player.Avatar;

            if (UnitType.ToString().StartsWith("400"))
            {
                CombatItemData _TroopData = (CombatItemData)CSVManager.DataTables.GetDataById(UnitType);
                ResourceData _TrainingResource = _TroopData.GetTrainingResource();

                if (_TroopData != null)
                {
                    Slot _DataSlot = _Player.Units.Find(t => t.Data == _TroopData.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Count += this.Count;
                    }
                    else
                    {
                        Slot ds = new Slot(_TroopData.GetGlobalID(), this.Count);
                        _Player.Units.Add(ds);
                    }

                    _Player.Resources.Minus(_TrainingResource.GetGlobalID(), _TroopData.GetTrainingCost(_Player.GetUnitUpgradeLevel(_TroopData)));
                }
            }
            else if (UnitType.ToString().StartsWith("260"))
            {
                SpellData _SpellData = (SpellData)CSVManager.DataTables.GetDataById(UnitType);
                ResourceData _CastResource = _SpellData.GetTrainingResource();

                if (_SpellData != null)
                {
                    Slot _DataSlot = _Player.Spells.Find(t => t.Data == _SpellData.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Count += this.Count;
                    }
                    else
                    {
                        Slot ds = new Slot(_SpellData.GetGlobalID(), this.Count);
                        _Player.Spells.Add(ds);
                    }

                    _Player.Resources.Minus(_CastResource.GetGlobalID(), _SpellData.GetTrainingCost(_Player.GetUnitUpgradeLevel(_SpellData)));
                }
            }
        }
    }
}

