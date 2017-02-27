using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 600
    internal class PlaceAttackerCommand : Command
    {
        public PlaceAttackerCommand(PacketReader br)
        {
            X    = br.ReadInt32WithEndian();
            Y    = br.ReadInt32WithEndian();
            Unit = (CombatItemData)br.ReadDataReference(); ; 
            Tick = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            if (level.GetPlayerAvatar().State != ClientAvatar.UserState.CHA)
            {
                List<DataSlot> _PlayerUnits = level.GetPlayerAvatar().GetUnits();

                DataSlot _DataSlot = _PlayerUnits.Find(t => t.Data.GetGlobalID() == Unit.GetGlobalID());
                if (_DataSlot != null)
                {
                    _DataSlot.Value = _DataSlot.Value - 1;
                }
            }
        }

        public CombatItemData Unit { get; set; }
        public uint Tick { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}