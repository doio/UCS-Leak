using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 509
    internal class CancelUnitProductionCommand : Command
    {
        public CancelUnitProductionCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.Unknown1 = this.Reader.ReadUInt32();
            this.UnitType = this.Reader.ReadInt32();
            this.Count = this.Reader.ReadInt32();
            this.Unknown3 = this.Reader.ReadUInt32();
            this.Unknown4 = this.Reader.ReadUInt32();
        }

        internal override void Process()
        {
            var go = this.Device.Player.GameObjectManager.GetGameObjectByID(BuildingId);
            if (Count > 0)
            {
                var b = (Building) go;
                var c = b.GetUnitProductionComponent();
                var cd = (CombatItemData)CSVManager.DataTables.GetDataById(UnitType);
                do
                {
                    c.RemoveUnit(cd);
                    Count--;
                }
                while (Count > 0);
            }
        }

        public int BuildingId;
        public int Count;
        public int UnitType;
        public uint Unknown1;
        public uint Unknown3;
        public uint Unknown4;
    }
}