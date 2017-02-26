using System.Collections.Generic;
using System.IO;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands
{
    // Packet 604
    internal class CastSpellCommand : Command
    {
        public CastSpellCommand(PacketReader br)
        {
            X        = br.ReadInt32WithEndian();
            Y        = br.ReadInt32WithEndian();
            Spell    = (SpellData) br.ReadDataReference();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            List<DataSlot> _PlayerSpells = level.GetPlayerAvatar().GetSpells();

            DataSlot _DataSlot = _PlayerSpells.Find(t => t.Data.GetGlobalID() == Spell.GetGlobalID());
            if (_DataSlot != null)
            {
                _DataSlot.Value = _DataSlot.Value - 1;
            }
        }

        public SpellData Spell { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}