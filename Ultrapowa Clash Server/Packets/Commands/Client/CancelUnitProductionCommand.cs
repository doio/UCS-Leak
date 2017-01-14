using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 509
    internal class CancelUnitProductionCommand : Command
    {
        public CancelUnitProductionCommand(PacketReader br)
        {
            BuildingId = br.ReadInt32WithEndian(); 
            Unknown1 = br.ReadUInt32WithEndian();
            UnitType = br.ReadInt32WithEndian();
            Count = br.ReadInt32WithEndian();
            Unknown3 = br.ReadUInt32WithEndian();
            Unknown4 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
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

        public int BuildingId { get; set; }
        public int Count { get; set; }
        public int UnitType { get; set; }
        public uint Unknown1 { get; set; } 
        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
    }
}