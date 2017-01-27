using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeAttackMessage : Message
    {
        public ChallangeAttackMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public long ID { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                ID = br.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
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

            Alliance a = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            Level defender = ResourcesManager.GetPlayer(a.GetChatMessages().Find(c => c.GetId() == ID).GetSenderId());
            if (defender != null)
            {
                defender.Tick();
                new ChallangeAttackDataMessage(Client, defender).Send();
            }
            else
            {
                new OwnHomeDataMessage(Client, level);
            }
            
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            StreamEntry s = alliance.GetChatMessages().Find(c => c.GetStreamEntryType() == 12);
            if (s != null)
            {
                alliance.GetChatMessages().RemoveAll(t => t == s);

                foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                {
                    Level playera = ResourcesManager.GetPlayer(op.GetAvatarId());
                    if (playera.GetClient() != null)
                    {
                        AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(playera.GetClient());
                        p.SetStreamEntry(s);
                        p.Send();
                    }
                }
            }
        }
    }
}
