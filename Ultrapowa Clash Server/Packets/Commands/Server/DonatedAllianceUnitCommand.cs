using System;
using System.Collections.Generic;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Server
{
    internal class DonatedAllianceUnitCommand : Command
    {
        public DonatedAllianceUnitCommand()
        {
        }

        public override byte[] Encode()
        {
            List<byte> _Data = new List<byte>();
            _Data.AddString(Donator);
            _Data.AddInt32(0);
            _Data.AddInt32(TroopID);
            _Data.AddInt32(TroopLevel);
            _Data.AddInt32(1);
            _Data.AddInt32(0);
            return _Data.ToArray();
        }

        public string Donator { get; set; }

        public int TroopLevel { get; set; }

        public int TroopID { get; set; }

        public int SetUnitLevel(int t) => TroopLevel = t;

        public int SetUnitID(int i) => TroopID = i;

        public void SetDonator(string name) => Donator = name;

        public void Tick(Level level) => level.Tick();
    }
}
