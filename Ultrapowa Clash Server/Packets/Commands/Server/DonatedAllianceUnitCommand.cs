using System;
using System.Collections.Generic;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Server
{
    internal class DonatedAllianceUnitCommand : Command
    {
        public int MesasgeID { get; set; }

        public CombatItemData TroopID { get; set; }

        public DonatedAllianceUnitCommand()
        {
        }

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(0);
            data.AddInt32(MesasgeID);
            data.AddInt32(0);
            data.AddInt32(TroopID.GetGlobalID()); //TypeID
            data.AddInt32(4);
            data.AddInt32(0); //Tick
            return data.ToArray();
        }

        public void DonationMessageID(int i)
        {
            MesasgeID = i;
        }

        public void Troop(CombatItemData i)
        {
            TroopID = i;
        }

        public void Tick(Level level)
        {
            level.Tick();
        }
    }
}
