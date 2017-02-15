using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    class RetributionAttackerMessage : Message
    {
        public RetributionAttackerMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override async void Process(Level level)
        {
            ClientAvatar p = level.GetPlayerAvatar();
            if (p.State == ClientAvatar.UserState.PVE || p.State == ClientAvatar.UserState.PVP)
            {
                ResourcesManager.DisconnectClient(Client);
            }
            else
            {
                if (level.GetPlayerAvatar().GetUnits().Count < 10)
                {
                    for (int i = 0; i < 31; i++)
                    {
                        Data unitData = CSVManager.DataTables.GetDataById(4000000 + i);
                        CharacterData combatData = (CharacterData)unitData;
                        int maxLevel = combatData.GetUpgradeLevelCount();
                        DataSlot unitSlot = new DataSlot(unitData, 1000);

                        level.GetPlayerAvatar().GetUnits().Add(unitSlot);
                        level.GetPlayerAvatar().SetUnitUpgradeLevel(combatData, maxLevel - 1);
                    }

                    for (int i = 0; i < 18; i++)
                    {
                        Data spellData = CSVManager.DataTables.GetDataById(26000000 + i);
                        SpellData combatData = (SpellData)spellData;
                        int maxLevel = combatData.GetUpgradeLevelCount();
                        DataSlot spellSlot = new DataSlot(spellData, 1000);

                        level.GetPlayerAvatar().GetSpells().Add(spellSlot);
                        level.GetPlayerAvatar().SetUnitUpgradeLevel(combatData, maxLevel - 1);
                    }
                }
                p.State = ClientAvatar.UserState.PVE;
                PacketProcessor.Send(new RetributionDataMessage(Client, level, 17000049));
            }
        }
    }
}
