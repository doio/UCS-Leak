using UCS.Helpers.Binary;
using UCS.Logic.JSONProperty;

namespace UCS.Packets.Commands.Client
{
    internal class TrainQuickUnitsCommand : Command
    {
        public TrainQuickUnitsCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {

        }

        internal override void Decode()
        {
            this.DataSlotID = this.Reader.ReadInt32();
            this.Tick = this.Reader.ReadInt32();
        }

        public int DataSlotID;
        public int Tick;

        internal override void Process()
        {
            var player = this.Device.Player.Avatar;

            if (DataSlotID == 1)
            {
                foreach (Slot i in player.QuickTrain1)
                {
                    Slot _DataSlot = player.Units.Find(t => t.Data == i.Data);
                    if (_DataSlot != null)
                    {
                        _DataSlot.Count = _DataSlot.Count + i.Count;
                    }
                    else
                    {
                        Slot ds = new Slot(i.Data, i.Count);
                        this.Device.Player.Avatar.Units.Add(ds);
                    }
                }
            }
            else if (DataSlotID == 2)
            {
                foreach (Slot i in player.QuickTrain2)
                {
                    Slot _DataSlot = player.Units.Find(t => t.Data == i.Data);
                    if (_DataSlot != null)
                    {
                        _DataSlot.Count = _DataSlot.Count + i.Count;
                    }
                    else
                    {
                        Slot ds = new Slot(i.Data, i.Count);
                        player.Units.Add(ds);
                    }
                }
            }
            else if (DataSlotID == 3)
            {
                foreach (Slot i in player.QuickTrain3)
                {
                    Slot _DataSlot = player.Units.Find(t => t.Data == i.Data);
                    if (_DataSlot != null)
                    {
                        _DataSlot.Count = _DataSlot.Count + i.Count;
                    }
                    else
                    {
                        Slot ds = new Slot(i.Data, i.Count);
                        player.Units.Add(ds);
                    }
                }
            }
        }
    }
}
