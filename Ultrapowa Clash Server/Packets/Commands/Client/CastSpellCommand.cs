using System.IO;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 604
    internal class CastSpellCommand : Command
    {
        public CastSpellCommand(PacketReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            Spell = (SpellData) br.ReadDataReference();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var components = level.GetComponentManager().GetComponents(0);
            for (var i = 0; i < components.Count; i++)
            {
                var c = (UnitStorageComponent) components[i];
                if (c.GetUnitTypeIndex(Spell) != -1)
                {
                    var storageCount = c.GetUnitCountByData(Spell);
                    if (storageCount >= 1)
                    {
                        c.RemoveUnits(Spell, 1);
                        break;
                    }
                }
            }
        }

        public SpellData Spell { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}