using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.JSONProperty.Item;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 603
    internal class EndOfBattleCommand : Command
    {
        internal int Tick;
        public EndOfBattleCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }


        internal override void Decode()
        {
            this.Tick = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            if (this.Device.PlayerState == Logic.Enums.State.IN_BATTLE)
            {
                Battle_Command Command = new Battle_Command
                {
                    Command_Type = this.Identifier,
                    Command_Base = new Command_Base { Base = new Base { Tick = this.Tick } }
                };
                Core.ResourcesManager.Battles[this.Device.Player.Avatar.BattleId].Add_Command(Command);
            }
            else
            {
                new OutOfSyncMessage(this.Device).Send();

            }
        }
    }
}