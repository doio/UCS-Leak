using System.Collections.Generic;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic.Enums;
using UCS.Logic.JSONProperty;

namespace UCS.Packets.Commands.Client
{
    // Packet 600
    internal class PlaceAttackerCommand : Command
    {
        public PlaceAttackerCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.Unit = (CombatItemData)this.Reader.ReadDataReference();
            this.Tick = this.Reader.ReadInt32();
        }


        internal override void Process()
        {

            if (this.Device.PlayerState == State.IN_BATTLE)
            {/*
                if (this.Device.Player.Avatar.Battle_ID > 0)
                {
                    Battle_Command Command = new Battle_Command
                    {
                        Command_Type = this.Identifier,
                        Command_Base =
                            new Command_Base
                            {
                                Base = new Logic.Items.Base { Tick = this.Tick },
                                Data = this.Unit.GetGlobalID(),
                                X = this.X,
                                Y = this.Y
                            }
                    };

                    ResourcesManager.GetInMemoryBattle(this.Device.Player.Avatar.Battle_ID).Add_Command(Command);

                    int Index = ResourcesManager.GetInMemoryBattle(this.Device.Player.Avatar.Battle_ID).Replay_Info.Units.FindIndex(T => T[0] == this.Unit.GetGlobalID());
                    if (Index > -1)
                        ResourcesManager.GetInMemoryBattle(this.Device.Player.Avatar.Battle_ID).Replay_Info.Units[Index][1]++;
                    else
                        ResourcesManager.GetInMemoryBattle(this.Device.Player.Avatar.Battle_ID).Replay_Info.Units.Add(new[] { this.Unit.GetGlobalID(), 1 });
                }
                */
                List<Slot> _PlayerUnits = this.Device.Player.Avatar.Units;

                Slot _DataSlot = _PlayerUnits.Find(t => t.Data == Unit.GetGlobalID());
                if (_DataSlot != null)
                {
                    _DataSlot.Count -= 1;
                }
            }
        }

        public CombatItemData Unit;
        public int Tick;
        public int X;
        public int Y;
    }
}