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
				foreach (var i in player.QuickTrain1)
				{
					DataSlot ds = new DataSlot(i.Data, i.Value);
					units.Add(ds);
				}
			}
			else if (DataSlotID == 2)
			{
				foreach (var i in player.QuickTrain2)
				{
					DataSlot ds = new DataSlot(i.Data, i.Value);
					units.Add(ds);
				}
			}
			else if (DataSlotID == 3)
			{
				foreach (var i in player.QuickTrain3)
				{
					DataSlot ds = new DataSlot(i.Data, i.Value);
					units.Add(ds);
				}
			}			
		}
	}
}
