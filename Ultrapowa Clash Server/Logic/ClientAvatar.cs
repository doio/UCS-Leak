using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using static System.Convert;
using static System.Configuration.ConfigurationManager;
using UCS.Logic.DataSlots;
using System.Threading.Tasks;
using UCS.Helpers.List;

namespace UCS.Logic
{
    internal class ClientAvatar : Avatar
    {
        // Long
        internal long AllianceId;
        internal long CurrentHomeId;
        internal long UserId;

        // Int
        internal int HighID;
        internal int LowID;
        private int m_vAvatarLevel;
        private int m_vCurrentGems;
        private int m_vExperience;
        private int m_vLeagueId;
        private int m_vScore;          
        private int m_vDonatedUnits;
        private int m_vRecievedUnits;
        private int m_vActiveLayout;
        private int m_vAlliance_Gold       = 2800000;
        private int m_vAlliance_Elixir     = 2800000;
        private int m_vAlliance_DarkElixir = 14400;
        private int m_vShieldTime;
        private int m_vProtectionTime;
        public int ReportedTimes           = 0;
        private int m_vDonated;
        private int m_vReceived;

        // Byte
        private byte m_vNameChangingLeft;
        private byte m_vnameChosenByUser;
        internal byte AccountPrivileges;
        // String
        internal string AvatarName;
        internal string UserToken;
        internal string Region;
        internal string FacebookId;
        internal string FacebookToken;
        internal string GoogleId;
        internal string GoogleToken;
        internal string IPAddress;

        // Boolean
        private bool m_vPremium;
        private bool m_vAndroid;
        internal bool AccountBanned;
        //Datetime
        private DateTime m_vAccountCreationDate;
        internal DateTime LastTickSaved;

        public struct AttackInfo
        {
            public Level Defender;
            public Level Attacker;

            public int Lost;
            public int Reward;

            public List<DataSlot> UsedTroop;
        }

        public ClientAvatar()
        {
            Achievements         = new List<DataSlot>();
            AchievementsUnlocked = new List<DataSlot>();
            AllianceUnits        = new List<DonationSlot>();
            NpcStars             = new List<DataSlot>();
            NpcLootedGold        = new List<DataSlot>();
            NpcLootedElixir      = new List<DataSlot>();
            BookmarkedClan       = new List<BookmarkSlot>();
            QuickTrain1          = new List<DataSlot>();
            QuickTrain2          = new List<DataSlot>();
            QuickTrain3          = new List<DataSlot>();
            AttackingInfo        = new Dictionary<long, AttackInfo>();
        }

        public ClientAvatar(long id, string token) : this()
        {
            Random rnd             = new Random();
            LastUpdate             = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            Login                  = id.ToString() + (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            this.UserId                  = id;
            this.HighID            = (int)(id >> 32);
            this.LowID             = (int)(id & 0xffffffffL);
            this.UserToken         = token;
            this.CurrentHomeId     = id;
            this.AccountPrivileges = 0;
            this.AccountBanned    = false;
            m_vnameChosenByUser    = 0x00;
            m_vNameChangingLeft    = 0x02;
            m_vAvatarLevel         = ToInt32(AppSettings["startingLevel"]);
            this.AllianceId        = 0;
            m_vExperience          = 0;
            EndShieldTime          = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            m_vCurrentGems         = ToInt32(AppSettings["startingGems"]);
            m_vScore               = AppSettings["startingTrophies"] == "random"
                ? rnd.Next(1500, 4999)
                : ToInt32(AppSettings["startingTrophies"]);

            TutorialStepsCount     = 0x0A;
            m_vPremium             = false;
            this.AvatarName        = "NoNameYet";

            SetResourceCount(CSVManager.DataTables.GetResourceByName("Gold"), ToInt32(AppSettings["startingGold"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("Elixir"), ToInt32(AppSettings["startingElixir"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("DarkElixir"), ToInt32(AppSettings["startingDarkElixir"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("Diamonds"), ToInt32(AppSettings["startingGems"]));
        }

        public List<DataSlot> Achievements { get; set; }
        public List<DataSlot> AchievementsUnlocked { get; set; }
        public List<DonationSlot> AllianceUnits { get; set; }
        public int EndShieldTime { get; set; }
        public int LastUpdate { get; set; }
        public string Login { get; set; }
        public List<DataSlot> NpcLootedElixir { get; set; }
        public List<DataSlot> NpcLootedGold { get; set; }
        public List<DataSlot> NpcStars { get; set; }
        public List<BookmarkSlot> BookmarkedClan { get; set; }
        public Dictionary<long, AttackInfo> AttackingInfo { get; set; }
        public List<DataSlot> QuickTrain1 { get; set; }
        public List<DataSlot> QuickTrain2 { get; set; }
        public List<DataSlot> QuickTrain3 { get; set; }

        void updateLeague()
        {
            var table = CSVManager.DataTables.GetTable(12);
            int i = 0;
            bool found = false;
            while (!found)
            {
                var league = (LeagueData)table.GetItemAt(i);
                if (m_vScore <= league.BucketPlacementRangeHigh[league.BucketPlacementRangeHigh.Count - 1] &&
                    m_vScore >= league.BucketPlacementRangeLow[0])
                {
                    found = true;
                    SetLeagueId(i);
                }
                i++;
            }
        }

        public uint TutorialStepsCount { get; set; }

        public void AddDiamonds(int diamondCount)
        {
            m_vCurrentGems += diamondCount;
        }

        public void AddExperience(int exp)
        {
            m_vExperience += exp;
            var experienceCap =
                ((ExperienceLevelData)CSVManager.DataTables.GetTable(10).GetDataByName(m_vAvatarLevel.ToString()))
                    .ExpPoints;
            if (m_vExperience >= experienceCap)
                if (CSVManager.DataTables.GetTable(10).GetItemCount() > m_vAvatarLevel + 1)
                {
                    m_vAvatarLevel += 1;
                    m_vExperience = m_vExperience - experienceCap;
                }
                else
                    m_vExperience = 0;
        }

        public async Task<byte[]> Encode()
        {
            try
            {
                Random rnd = new Random();
                List<byte> data = new List<byte>();
                data.AddLong(this.UserId);
                data.AddLong(this.CurrentHomeId);
                if (this.AllianceId != 0)
                {
                    data.Add(1);
                    data.AddLong(this.AllianceId);
                    Alliance alliance = await ObjectManager.GetAlliance(this.AllianceId);
                    data.AddString(alliance.GetAllianceName());
                    data.AddInt(alliance.GetAllianceBadgeData());
                    data.AddInt(alliance.GetAllianceMember(this.UserId).GetRole());
                    data.AddInt(alliance.GetAllianceLevel());
                }
                data.Add(0);

                if (m_vLeagueId == 22)
                {
                    data.AddInt(m_vScore / 12);
                    data.AddInt(1);
                    int month = DateTime.Now.Month;
                    data.AddInt(month);
                    data.AddInt(DateTime.Now.Year);
                    data.AddInt(rnd.Next(1, 10));
                    data.AddInt(m_vScore);
                    data.AddInt(1);
                    if (month == 1)
                    {
                        data.AddInt(12);
                        data.AddInt(DateTime.Now.Year - 1);
                    }
                    else
                    {
                        int pmonth = month - 1;
                        data.AddInt(pmonth);
                        data.AddInt(DateTime.Now.Year);
                    }
                    data.AddInt(rnd.Next(1, 10));
                    data.AddInt(m_vScore / 2);
                }
                else
                {
                    data.AddInt(0); //1
                    data.AddInt(0); //2
                    data.AddInt(0); //3
                    data.AddInt(0); //4
                    data.AddInt(0); //5
                    data.AddInt(0); //6
                    data.AddInt(0); //7
                    data.AddInt(0); //8
                    data.AddInt(0); //9
                    data.AddInt(0); //10
                    data.AddInt(0); //11
                }

                data.AddInt(m_vLeagueId);
                data.AddInt(GetAllianceCastleLevel());
                data.AddInt(GetAllianceCastleTotalCapacity());
                data.AddInt(GetAllianceCastleUsedCapacity());
                data.AddInt(0);
                data.AddInt(-1);
                data.AddInt(GetTownHallLevel());
                data.AddString(this.AvatarName);
                data.AddString(this.FacebookId);
                data.AddInt(m_vAvatarLevel);
                data.AddInt(m_vExperience);
                data.AddInt(m_vCurrentGems);
                data.AddInt(m_vCurrentGems);
                data.AddInt(1200);
                data.AddInt(60);
                data.AddInt(m_vScore);
                data.AddInt(200); // Attack Wins
                data.AddInt(m_vDonated);
                data.AddInt(100); // Attack Loses
                data.AddInt(m_vReceived);

                data.AddInt(m_vAlliance_Gold);
                data.AddInt(m_vAlliance_Elixir);
                data.AddInt(m_vAlliance_DarkElixir);
                data.AddInt(0);
                data.Add(1);
                data.AddLong(946720861000);

                data.Add(m_vnameChosenByUser);

                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(1);

                data.AddInt(0);
                data.AddInt(0);
                data.Add(0);
                data.AddDataSlots(GetResourceCaps());
                data.AddDataSlots(GetResources());
                data.AddDataSlots(GetUnits());
                data.AddDataSlots(GetSpells());
                data.AddDataSlots(m_vUnitUpgradeLevel);
                data.AddDataSlots(m_vSpellUpgradeLevel);
                data.AddDataSlots(m_vHeroUpgradeLevel);
                data.AddDataSlots(m_vHeroHealth);
                data.AddDataSlots(m_vHeroState);

                data.AddRange(BitConverter.GetBytes(AllianceUnits.Count).Reverse());
                foreach (DonationSlot u in AllianceUnits)
                {
                    data.AddInt(u.ID);
                    data.AddInt(u.Count);
                    data.AddInt(u.UnitLevel);
                }

                data.AddRange(BitConverter.GetBytes(TutorialStepsCount).Reverse());
                for (uint i = 0; i < TutorialStepsCount; i++)
                    data.AddRange(BitConverter.GetBytes(0x01406F40 + i).Reverse());

                data.AddRange(BitConverter.GetBytes(Achievements.Count).Reverse());
                foreach (var a in Achievements)
                    data.AddRange(BitConverter.GetBytes(a.Data.GetGlobalID()).Reverse());

                data.AddRange(BitConverter.GetBytes(Achievements.Count).Reverse());
                foreach (var a in Achievements)
                {
                    data.AddRange(BitConverter.GetBytes(a.Data.GetGlobalID()).Reverse());
                    data.AddRange(BitConverter.GetBytes(0).Reverse());
                }

                data.AddRange(BitConverter.GetBytes(ObjectManager.NpcLevels.Count).Reverse());
                {
                    for (var i = 17000000; i < 17000050; i++)
                    {
                        data.AddRange(BitConverter.GetBytes(i).Reverse());
                        data.AddRange(BitConverter.GetBytes(rnd.Next(3, 3)).Reverse());
                    }
                }

                data.AddDataSlots(NpcLootedGold);
                data.AddDataSlots(NpcLootedElixir);
                data.AddDataSlots(new List<DataSlot>());
                data.AddDataSlots(new List<DataSlot>());
                data.AddDataSlots(new List<DataSlot>());
                data.AddDataSlots(new List<DataSlot>());

                data.AddInt(QuickTrain1.Count);
                foreach (var i in QuickTrain1)
                {
                    data.AddInt(i.Data.GetGlobalID());
                    data.AddInt(i.Value);
                }

                data.AddInt(QuickTrain2.Count);
                foreach (var i in QuickTrain2)
                {
                    data.AddInt(i.Data.GetGlobalID());
                    data.AddInt(i.Value);
                }
                data.AddInt(QuickTrain3.Count);
                foreach (var i in QuickTrain3)
                {
                    data.AddInt(i.Data.GetGlobalID());
                    data.AddInt(i.Value);
                }
                data.AddInt(QuickTrain1.Count);
                foreach (var i in QuickTrain1)
                {
                    data.AddInt(i.Data.GetGlobalID());
                    data.AddInt(i.Value);
                }
                data.AddDataSlots(new List<DataSlot>());
                return data.ToArray();
            } catch (Exception) { return null; }
        }

        public long GetAllianceId() => this.AllianceId;

        public async Task<AllianceMemberEntry> GetAllianceMemberEntry()
        {
            try
            {
                var alliance = await ObjectManager.GetAlliance(this.AllianceId);
                return alliance?.GetAllianceMember(this.UserId);
            } catch (Exception) { return null; }
        }

        public DateTime GetAccountCreationDate() => m_vAccountCreationDate;

        public async Task<int> GetAllianceRole()
        {
            try
            {
                var ame = await GetAllianceMemberEntry();
                if (ame != null)
                    return ame.GetRole();
                return -1;
            } catch (Exception) { return 1; }
        }

        public int GetAlliance_Gold() => m_vAlliance_Gold;

        public int GetAlliance_Elixir() => m_vAlliance_Elixir;

        public int GetAlliance_DarkElixir() => m_vAlliance_DarkElixir;

        public int GetAvatarLevel() => m_vAvatarLevel;

        public int GetActiveLayout() =>  m_vActiveLayout;

        public long GetCurrentHomeId() => CurrentHomeId;

        public int GetDiamonds() => m_vCurrentGems;

        public bool GetAndroid() => m_vAndroid;

        public bool GetPremium() => m_vPremium;

        public long GetId() => UserId;

        public int GetShieldTime => m_vShieldTime;

        public int GetProtectionTime => m_vProtectionTime;

        public int GetLeagueId() => m_vLeagueId;

        public int GetScore()
        {
            updateLeague();
            return m_vScore;
        }

        public int GetSecondsFromLastUpdate() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - LastUpdate;

        public int GetDonated() => m_vDonated;

        public int GetReceived() => m_vReceived;

        public bool HasEnoughDiamonds(int diamondCount) => m_vCurrentGems >= diamondCount;

        public bool HasEnoughResources(ResourceData rd, int buildCost) => GetResourceCount(rd) >= buildCost;

        public void LoadFromJSON(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);
            this.UserId = jsonObject["avatar_id"].ToObject<long>();
            this.HighID = jsonObject["id_high_int"].ToObject<int>();
            this.LowID = jsonObject["id_low_int"].ToObject<int>();
            this.UserToken = jsonObject["token"].ToObject<string>();
            this.Region = jsonObject["region"].ToObject<string>();
            this.IPAddress = jsonObject["IPAddress"].ToObject<string>();
            m_vAccountCreationDate = jsonObject["avatar_creation_date"].ToObject<DateTime>();
            this.AccountPrivileges = jsonObject["avatar_privilages"].ToObject<byte>();
            this.AccountBanned = jsonObject["avatar_banned"].ToObject<bool>();
            m_vActiveLayout = jsonObject["active_layout"].ToObject<int>();
            this.LastTickSaved = jsonObject["last_tick_save"].ToObject<DateTime>();
            m_vAndroid = jsonObject["android"].ToObject<bool>();
            this.CurrentHomeId = jsonObject["current_home_id"].ToObject<long>();
            this.AllianceId = jsonObject["alliance_id"].ToObject<long>();
            SetAllianceCastleLevel(jsonObject["alliance_castle_level"].ToObject<int>());
            SetAllianceCastleTotalCapacity(jsonObject["alliance_castle_total_capacity"].ToObject<int>());
            SetAllianceCastleUsedCapacity(jsonObject["alliance_castle_used_capacity"].ToObject<int>());
            SetTownHallLevel(jsonObject["townhall_level"].ToObject<int>());
            this.AvatarName = jsonObject["avatar_name"].ToObject<string>();
            m_vAvatarLevel = jsonObject["avatar_level"].ToObject<int>();
            m_vExperience = jsonObject["experience"].ToObject<int>();
            m_vCurrentGems = jsonObject["current_gems"].ToObject<int>();
            SetScore(jsonObject["score"].ToObject<int>());
            m_vNameChangingLeft = jsonObject["nameChangesLeft"].ToObject<byte>();
            m_vnameChosenByUser = jsonObject["nameChosenByUser"].ToObject<byte>();
            m_vShieldTime = jsonObject["shield_time"].ToObject<int>();
            m_vProtectionTime = jsonObject["protection_time"].ToObject<int>();
            this.FacebookId = jsonObject["fb_id"].ToObject<string>();
            this.FacebookToken = jsonObject["fb_token"].ToObject<string>();
            this.GoogleId = jsonObject["gg_id"].ToObject<string>();
            this.GoogleToken = jsonObject["gg_token"].ToObject<string>();
            var jsonBookmarkedClan = (JArray)jsonObject["bookmark"];
            foreach (JObject jobject in jsonBookmarkedClan)
            {
                JObject data = (JObject)jobject;
                BookmarkSlot ds = new BookmarkSlot(0);
                ds.Load(data);
                BookmarkedClan.Add(ds);
            }

            var jsonResources = (JArray) jsonObject["resources"];
            foreach (JObject resource in jsonResources)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(resource);
                GetResources().Add(ds);
            }

            var jsonUnits = (JArray) jsonObject["units"];
            foreach (JObject unit in jsonUnits)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(unit);
                m_vUnitCount.Add(ds);
            }

            var jsonSpells = (JArray) jsonObject["spells"];
            foreach (JObject spell in jsonSpells)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(spell);
                m_vSpellCount.Add(ds);
            }

            var jsonUnitLevels = (JArray) jsonObject["unit_upgrade_levels"];
            foreach (JObject unitLevel in jsonUnitLevels)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(unitLevel);
                m_vUnitUpgradeLevel.Add(ds);
            }

            var jsonSpellLevels = (JArray) jsonObject["spell_upgrade_levels"];
            foreach (JObject data in jsonSpellLevels)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vSpellUpgradeLevel.Add(ds);
            }

            var jsonHeroLevels = (JArray) jsonObject["hero_upgrade_levels"];
            foreach (JObject data in jsonHeroLevels)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroUpgradeLevel.Add(ds);
            }

            var jsonHeroHealth = (JArray) jsonObject["hero_health"];
            foreach (JObject data in jsonHeroHealth)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroHealth.Add(ds);
            }

            var jsonHeroState = (JArray) jsonObject["hero_state"];
            foreach (JObject data in jsonHeroState)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroState.Add(ds);
            }

            var jsonAllianceUnits = (JArray) jsonObject["alliance_units"];
            foreach (JObject data in jsonAllianceUnits)
            {
                DonationSlot ds = new DonationSlot(0, 0, 0, 0);
                ds.Load(data);
                AllianceUnits.Add(ds);
            }
            TutorialStepsCount = jsonObject["tutorial_step"].ToObject<uint>();

            var jsonAchievementsProgress = (JArray) jsonObject["achievements_progress"];
            foreach (JObject data in jsonAchievementsProgress)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                Achievements.Add(ds);
            }

            var jsonNpcStars = (JArray) jsonObject["npc_stars"];
            foreach (JObject data in jsonNpcStars)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcStars.Add(ds);
            }

            var jsonNpcLootedGold = (JArray) jsonObject["npc_looted_gold"];
            foreach (JObject data in jsonNpcLootedGold)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcLootedGold.Add(ds);
            }

            var jsonNpcLootedElixir = (JArray) jsonObject["npc_looted_elixir"];
            foreach (JObject data in jsonNpcLootedElixir)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcLootedElixir.Add(ds);
            }
            var jsonQuickTrain1 = (JArray)jsonObject["quick_train_1"];
            foreach (JObject data in jsonQuickTrain1)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                QuickTrain1.Add(ds);
            }
            var jsonQuickTrain2 = (JArray)jsonObject["quick_train_2"];
            foreach (JObject data in jsonQuickTrain2)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                QuickTrain2.Add(ds);
            }
            var jsonQuickTrain3 = (JArray)jsonObject["quick_train_3"];
            foreach (JObject data in jsonQuickTrain3)
            {
                DataSlot ds = new DataSlot(null, 0);
                ds.Load(data);
                QuickTrain3.Add(ds);
            }
            m_vPremium = jsonObject["Premium"].ToObject<bool>();
        }

        public string SaveToJSON()
        {
            #region Foreach Stuff
            JArray jsonBookmarkClan = new JArray();
            foreach (var clan in BookmarkedClan)
                jsonBookmarkClan.Add(clan.Save(new JObject()));

            JArray jsonResourcesArray = new JArray();
            foreach (var resource in GetResources())
                jsonResourcesArray.Add(resource.Save(new JObject()));

            JArray jsonUnitsArray = new JArray();
            foreach (var unit in GetUnits())
                jsonUnitsArray.Add(unit.Save(new JObject()));

            JArray jsonSpellsArray = new JArray();
            foreach (var spell in GetSpells())
                jsonSpellsArray.Add(spell.Save(new JObject()));

            JArray jsonUnitUpgradeLevelsArray = new JArray();
            foreach (var unitUpgradeLevel in m_vUnitUpgradeLevel)
                jsonUnitUpgradeLevelsArray.Add(unitUpgradeLevel.Save(new JObject()));


            JArray jsonSpellUpgradeLevelsArray = new JArray();
            foreach (var spellUpgradeLevel in m_vSpellUpgradeLevel)
                jsonSpellUpgradeLevelsArray.Add(spellUpgradeLevel.Save(new JObject()));

            JArray jsonHeroUpgradeLevelsArray = new JArray();
            foreach (var heroUpgradeLevel in m_vHeroUpgradeLevel)
                jsonHeroUpgradeLevelsArray.Add(heroUpgradeLevel.Save(new JObject()));

            JArray jsonHeroHealthArray = new JArray();
            foreach (var heroHealth in m_vHeroHealth)
                jsonHeroHealthArray.Add(heroHealth.Save(new JObject()));

            JArray jsonHeroStateArray = new JArray();
            foreach (var heroState in m_vHeroState)
                jsonHeroStateArray.Add(heroState.Save(new JObject()));

             JArray jsonAllianceUnitsArray = new JArray();
            foreach (var allianceUnit in AllianceUnits)
                jsonAllianceUnitsArray.Add(allianceUnit.Save(new JObject()));

            JArray jsonAchievementsProgressArray = new JArray();
            foreach (var achievement in Achievements)
                jsonAchievementsProgressArray.Add(achievement.Save(new JObject()));

            JArray jsonNpcStarsArray = new JArray();
            foreach (var npcLevel in NpcStars)
                jsonNpcStarsArray.Add(npcLevel.Save(new JObject()));

            JArray jsonNpcLootedGoldArray = new JArray();
            foreach (var npcLevel in NpcLootedGold)
                jsonNpcLootedGoldArray.Add(npcLevel.Save(new JObject()));
  
            JArray jsonNpcLootedElixirArray = new JArray();
            foreach (var npcLevel in NpcLootedElixir)
                jsonNpcLootedElixirArray.Add(npcLevel.Save(new JObject()));

            JArray jsonQuickTrain1Array = new JArray();
            foreach (var quicktrain1 in QuickTrain1)
                jsonQuickTrain1Array.Add(quicktrain1.Save(new JObject()));

            JArray jsonQuickTrain2Array = new JArray();
            foreach (var quicktrain2 in QuickTrain2)
                jsonQuickTrain1Array.Add(quicktrain2.Save(new JObject()));

            JArray jsonQuickTrain3Array = new JArray();
            foreach (var quicktrain3 in QuickTrain3)
                jsonQuickTrain3Array.Add(quicktrain3.Save(new JObject()));
        #endregion

            JObject jsonData = new JObject
            {
                {"avatar_id", this.UserId},
                {"id_high_int", this.HighID},
                {"id_low_int", this.LowID},
                {"token", this.UserToken},
                {"region", this.Region},
                {"IPAddress", this.IPAddress},
                {"avatar_creation_date", m_vAccountCreationDate},
                {"avatar_privilages", this.AccountPrivileges},
                {"avatar_banned", this.AccountBanned},
                {"active_layout", m_vActiveLayout},
                {"last_tick_save", this.LastTickSaved},
                {"android", m_vAndroid},
                {"current_home_id", this.CurrentHomeId},
                {"alliance_id", this.AllianceId},
                {"alliance_castle_level", GetAllianceCastleLevel()},
                {"alliance_castle_total_capacity", GetAllianceCastleTotalCapacity()},
                {"alliance_castle_used_capacity", GetAllianceCastleUsedCapacity()},
                {"townhall_level", GetTownHallLevel()},
                {"avatar_name", this.AvatarName},
                {"avatar_level", m_vAvatarLevel},
                {"experience", m_vExperience},
                {"current_gems", m_vCurrentGems},
                {"score", GetScore()},
                {"nameChangesLeft", m_vNameChangingLeft},
                {"nameChosenByUser", (ushort) m_vnameChosenByUser},
                {"shield_time", m_vShieldTime},
                {"protection_time", m_vProtectionTime},
                {"fb_id", this.FacebookId},
                {"fb_token", this.FacebookToken},
                {"gg_id", this.GoogleId},
                {"gg_token", this.GoogleToken},
                {"bookmark", jsonBookmarkClan},
                {"resources", jsonResourcesArray},
                {"units", jsonUnitsArray},
                {"spells", jsonSpellsArray},
                {"unit_upgrade_levels", jsonUnitUpgradeLevelsArray},
                {"spell_upgrade_levels", jsonSpellUpgradeLevelsArray},
                {"hero_upgrade_levels", jsonHeroUpgradeLevelsArray},
                {"hero_health", jsonHeroHealthArray},
                {"hero_state", jsonHeroStateArray},
                {"alliance_units", jsonAllianceUnitsArray},
                {"tutorial_step", TutorialStepsCount},
                {"achievements_progress", jsonAchievementsProgressArray},
                {"npc_stars", jsonNpcStarsArray},
                {"npc_looted_gold", jsonNpcLootedGoldArray},
                {"npc_looted_elixir", jsonNpcLootedElixirArray},
                {"quick_train_1", jsonQuickTrain1Array},
                {"quick_train_2", jsonQuickTrain2Array},
                {"quick_train_3", jsonQuickTrain3Array},
                {"Premium", m_vPremium}
            };

            return JsonConvert.SerializeObject(jsonData, Formatting.Indented);
        }

        public void InitializeAccountCreationDate() => m_vAccountCreationDate = DateTime.Now;

        public void AddUsedTroop(CombatItemData cid, int value)
        {
            /*if (State == UserState.PVP)
            { 
            var info = default(AttackInfo);
            if (!AttackingInfo.TryGetValue(GetId(), out info))
            {
                Logger.Write("Unable to obtain attack info.");
            }
            
            DataSlot e = info.UsedTroop.Find(t => t.Data.GetGlobalID() == cid.GetGlobalID());
                if (e != null)
                {
                    // Troops already exist.
                    int i = info.UsedTroop.IndexOf(e);
                    e.Value = e.Value + value;
                    info.UsedTroop[i] = e;
                }
                else
                {
                    DataSlot ds = new DataSlot(cid, value);
                    info.UsedTroop.Add(ds);
                }
            }*/
            //else
              //  Logger.Write("Unsuppored state! AddUsedTroop only for PVP for now.PVE Comming Soon");
        }

        public void AddAllianceTroop(long did, int id, int value, int level)
        {
            DonationSlot e = AllianceUnits.Find(t => t.ID == id && t.DonatorID == did && t.UnitLevel == level);
            if (e != null)
            {
                int i = AllianceUnits.IndexOf(e);
                e.Count = e.Count + value;
                AllianceUnits[i] = e;
            }
            else
            {
                DonationSlot ds = new DonationSlot(did, id, value, level);
                AllianceUnits.Add(ds);
            }
        }

        public void SetAchievment(AchievementData ad, bool finished)
        {
            var index = GetDataIndex(Achievements, ad);
            int value = finished ? 1 : 0;
            if (index != -1)
                Achievements[index].Value = value;
            else
            {
                DataSlot ds = new DataSlot(ad, value);
                Achievements.Add(ds);
            }
        }

        public void SetPremium(bool count) => m_vPremium = count;

        public void SetAlliance_Gold(int gold) => m_vAlliance_Gold = gold;

        public void SetAlliance_Elixir(int elixir) => m_vAlliance_Elixir = elixir;

        public void SetAlliance_DarkElixir(int drkelixir) => m_vAlliance_DarkElixir = drkelixir;

        public void SetShieldTime(int time) => m_vShieldTime = time;

        public void SetProtectionTime(int time) => m_vProtectionTime = time;

        public void SetAllianceId(long id) => this.AllianceId = id;

        public void SetActiveLayout(int layout) => m_vActiveLayout = layout;

        public void SetAndroid(bool android) => m_vAndroid = android;

        public async void SetAllianceRole(int a)
        {
            try
            {
                AllianceMemberEntry ame = await GetAllianceMemberEntry();
                if (ame != null)
                    ame.SetRole(a);
            }
            catch (Exception){}
        }

        public void SetDiamonds(int count) => m_vCurrentGems = count;

        public void SetLeagueId(int id) => m_vLeagueId = id;

        public void SetName(string name)
        {
            this.AvatarName = name;
            if (m_vnameChosenByUser == 0x01)
            {
                m_vNameChangingLeft = 0x01;
            }
            else
            {
                m_vNameChangingLeft = 0x02;
            }
            TutorialStepsCount = 0x0D;
        }

        public void SetScore(int newScore)
        {
            m_vScore = newScore;
            updateLeague();
        }

        public void SetAvatarLevel(int newlv) => m_vAvatarLevel = newlv;

        public void UseDiamonds(int diamondCount) => m_vCurrentGems -= diamondCount;
    }
}
