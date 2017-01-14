using System;
using System.Diagnostics;
using System.IO;
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
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            Unit = (CharacterData) br.ReadDataReference();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            Debug.Assert((int)level.GetPlayerAvatar().State >= 1 , "Command was executed when the level was in an incorrect state.");
            level.GetPlayerAvatar().AddUsedTroop(Unit, 1); //Deactive this when this funtion work again
            var components = level.GetComponentManager().GetComponents(0);
            for (var i = 0; i < components.Count; i++)
            {
                var c = (UnitStorageComponent) components[i];
                if (c.GetUnitTypeIndex(Unit) != -1)
                {
                    var storageCount = c.GetUnitCountByData(Unit);
                    if (storageCount >= 0)
                    {
                        //Thing not call here
                        Console.WriteLine("Im exist");
                        c.RemoveUnits(Unit, 1);
                        //level.GetPlayerAvatar().AddUsedTroop(Unit, 1); Active this when this funtion work again
                        break;
                    }
                }
            }
        }

        public CharacterData Unit { get; set; }
        public uint Unknown1 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}