using System.Collections.Generic;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic.JSONProperty;

namespace UCS.Packets.Commands.Client
{
    // Packet 604
    internal class CastSpellCommand : Command
    {
        public CastSpellCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.Spell = (SpellData)this.Reader.ReadDataReference();
            this.Unknown1 = this.Reader.ReadUInt32();
        }

        internal override void Process()
        {
            List<Slot> _PlayerSpells = this.Device.Player.Avatar.Spells;

            Slot _DataSlot = _PlayerSpells.Find(t => t.Data == Spell.GetGlobalID());
            if (_DataSlot != null)
            {
                _DataSlot.Count -= 1;
            }
        }

        public SpellData Spell;
        public uint Unknown1;
        public int X;
        public int Y;
    }
}