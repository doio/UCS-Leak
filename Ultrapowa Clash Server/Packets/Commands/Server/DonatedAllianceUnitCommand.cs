using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Commands.Server
{
    internal class DonatedAllianceUnitCommand : Command
    {
        public DonatedAllianceUnitCommand(Device client) : base(client)
        {
            this.Identifier = 4;
        }

        internal override void Encode()
        {
            this.Data.AddString(Donator);
            this.Data.AddInt(0);
            this.Data.AddInt(TroopID);
            this.Data.AddInt(TroopLevel);
            this.Data.AddInt(1);
            this.Data.AddInt(0);
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
