using System;
using System.Collections.Generic;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 700
    internal class SearchOpponentCommand : Command
    {
        public SearchOpponentCommand(PacketReader br)
        {
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {                 
            for (int i = 0; i < 31; i++)
            {
                Data unitData = CSVManager.DataTables.GetDataById(4000000 + i);
                CharacterData combatData = (CharacterData)unitData;
                Int32 maxLevel = combatData.GetUpgradeLevelCount();
                DataSlot unitSlot = new DataSlot(unitData, 1000);

                level.GetPlayerAvatar().GetUnits().Add(unitSlot);
                level.GetPlayerAvatar().SetUnitUpgradeLevel(combatData, maxLevel - 1);
            }

            for (int i = 0; i < 18; i++)
            {
                Data spellData = CSVManager.DataTables.GetDataById(26000000 + i);
                SpellData combatData = (SpellData)spellData;
                Int32 maxLevel = combatData.GetUpgradeLevelCount();
                DataSlot spellSlot = new DataSlot(spellData, 1000);

                level.GetPlayerAvatar().GetSpells().Add(spellSlot); 
                level.GetPlayerAvatar().SetUnitUpgradeLevel(combatData, maxLevel - 1);
            }

            /*level.GetPlayerAvatar().State = ClientAvatar.UserState.Searching;
            Level l = ObjectManager.GetRandomOnlinePlayer();

            if (l != null)
            {
                l.Tick();
                new EnemyHomeDataMessage(level.GetClient(), l, level).Send();
            } */

            // New Method
            Level Defender = ObjectManager.GetRandomOnlinePlayerWithoutShield();
            Defender.Tick();
            new EnemyHomeDataMessage(level.GetClient(), Defender, level).Send();  
        }
    }
}
