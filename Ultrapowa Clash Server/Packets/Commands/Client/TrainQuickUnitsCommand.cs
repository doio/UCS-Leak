using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
	internal class TrainQuickUnitsCommand : Command
	{
		public TrainQuickUnitsCommand(PacketReader br)
		{
			DataSlotID = br.ReadInt32(); 
			Tick = br.ReadInt32(); 
		}

		public int DataSlotID { get; set; }
		public int Tick { get; set; }

		public override void Execute(Level level)
		{
			var player = level.GetPlayerAvatar();
			var units = player.GetUnits();

			if (DataSlotID == 1)
			{
				foreach (DataSlot i in player.QuickTrain1)
				{
                    List<DataSlot> _PlayerUnits = level.GetPlayerAvatar().GetUnits();
                    DataSlot _DataSlot = _PlayerUnits.Find(t => t.Data.GetGlobalID() == i.Data.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value + i.Value;
                    }
                    else
                    {
                        DataSlot ds = new DataSlot(i.Data, i.Value);
                        _PlayerUnits.Add(ds);
                    }
                }
            }
			else if (DataSlotID == 2)
			{
				foreach (DataSlot i in player.QuickTrain2)
				{
                    List<DataSlot> _PlayerUnits = level.GetPlayerAvatar().GetUnits();
                    DataSlot _DataSlot = _PlayerUnits.Find(t => t.Data.GetGlobalID() == i.Data.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value + i.Value;
                    }
                    else
                    {
                        DataSlot ds = new DataSlot(i.Data, i.Value);
                        _PlayerUnits.Add(ds);
                    }
                }
			}
			else if (DataSlotID == 3)
			{
				foreach (DataSlot i in player.QuickTrain3)
				{
                    List<DataSlot> _PlayerUnits = level.GetPlayerAvatar().GetUnits();
                    DataSlot _DataSlot = _PlayerUnits.Find(t => t.Data.GetGlobalID() == i.Data.GetGlobalID());
                    if (_DataSlot != null)
                    {
                        _DataSlot.Value = _DataSlot.Value + i.Value;
                    }
                    else
                    {
                        DataSlot ds = new DataSlot(i.Data, i.Value);
                        _PlayerUnits.Add(ds);
                    }
                }
			}			
		}
	}
}
