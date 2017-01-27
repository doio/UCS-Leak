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

namespace UCS.Logic
{
    internal class ClientAvatar : Avatar
    {
        // Long
        long m_vAllianceId;
        long m_vCurrentHomeId;
        long m_vId;

        // Int
        int m_vHighInt;
        int m_vLowInt;
        int m_vAvatarLevel;
        int m_vCurrentGems;
        int m_vExperience;
        int m_vLeagueId;
        int m_vScore;          
        int m_vDonatedUnits;
        int m_vRecievedUnits;
        int m_vActiveLayout;
        int m_vAlliance_Gold;
        int m_vAlliance_Elixir;
        int m_vAlliance_DarkElixir;
        int m_vShieldTime;
        int m_vProtectionTime;

        // Byte
        byte m_vNameChangingLeft;
        byte m_vnameChosenByUser;

        // String
        string m_vAvatarName;
        string m_vToken;
        string m_vRegion;

        // Boolean
        bool m_vPremium;
        bool m_vAndroid;
        
        //Datetime
        DateTime m_vAccountCreationDate;

        public enum UserState : int
        {
            Home      = 0,
            Editmode  = 1,
        }

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
            AllianceUnits        = new List<TroopDataSlot>();
            NpcStars             = new List<DataSlot>();
            NpcLootedGold        = new List<DataSlot>();
            NpcLootedElixir      = new List<DataSlot>();
            BookmarkedClan       = new List<BookmarkSlot>();
            DonationSlot         = new List<DonationSlot>();
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
            m_vId                  = id;
            m_vHighInt             = (int)(id >> 32);
            m_vLowInt              = (int)(id & 0xffffffffL);
            m_vToken               = token;
            m_vCurrentHomeId       = id;
            m_vnameChosenByUser    = 0x00;
            m_vNameChangingLeft    = 0x02;
            m_vAvatarLevel         = ToInt32(AppSettings["startingLevel"]);
            m_vAllianceId          = 0;
            m_vExperience          = 0;
            m_vAlliance_Gold       = 2800000;
            m_vAlliance_Elixir     = 2800000;
            m_vAlliance_DarkElixir = 14400;
            EndShieldTime          = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            m_vCurrentGems         = ToInt32(AppSettings["startingGems"]);
            m_vScore               = AppSettings["startingTrophies"] == "random"
                ? rnd.Next(1500, 4999)
                : ToInt32(AppSettings["startingTrophies"]);

            TutorialStepsCount     = 0x0A;
            m_vPremium             = false;
            m_vAvatarName          = "NoNameYet";

            SetResourceCount(CSVManager.DataTables.GetResourceByName("Gold"), ToInt32(AppSettings["startingGold"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("Elixir"), ToInt32(AppSettings["startingElixir"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("DarkElixir"), ToInt32(AppSettings["startingDarkElixir"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("Diamonds"), ToInt32(AppSettings["startingGems"]));
        }

        public List<DataSlot> Achievements { get; set; }
        public List<DataSlot> AchievementsUnlocked { get; set; }
        public List<TroopDataSlot> AllianceUnits { get; set; }
        public int EndShieldTime { get; set; }
        public int LastUpdate { get; set; }
        public UserState State { get; set; }
        public string Login { get; set; }
        public List<DataSlot> NpcLootedElixir { get; set; }
        public List<DataSlot> NpcLootedGold { get; set; }
        public List<DataSlot> NpcStars { get; set; }
        public List<BookmarkSlot> BookmarkedClan { get; set; }
        public List<DonationSlot> DonationSlot { get; set; }
        public Dictionary<long, AttackInfo> AttackingInfo { get; set; }
        public List<DataSlot> QuickTrain1 { get; set; }
        public List<DataSlot> QuickTrain2 { get; set; }
        public List<DataSlot> QuickTrain3 { get; set; }


        public uint Region { get; set; }

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

        public byte[] Encode()
        {
            Random rnd = new Random();
            List<byte> data = new List<byte>();
            data.AddInt64(m_vId);
            data.AddInt64(m_vCurrentHomeId);
            if (m_vAllianceId != 0)
            {
                data.Add(1);
                data.AddInt64(m_vAllianceId);
                Alliance alliance = ObjectManager.GetAlliance(m_vAllianceId);
                data.AddString(alliance.GetAllianceName());
                data.AddInt32(alliance.GetAllianceBadgeData());
                data.AddInt32(alliance.GetAllianceMember(m_vId).GetRole());
                data.AddInt32(alliance.GetAllianceLevel());
            }
            data.Add(0);

            if (m_vLeagueId == 22)
            {
                data.AddInt32(m_vScore / 12); 
                data.AddInt32(1);
                int  month = DateTime.Now.Month;
                data.AddInt32(month); 
                data.AddInt32(DateTime.Now.Year); 
                data.AddInt32(rnd.Next(1, 10)); 
                data.AddInt32(m_vScore); 
                data.AddInt32(1); 
                if (month == 1)
                {
                    data.AddInt32(12); 
                    data.AddInt32(DateTime.Now.Year - 1); 
                }
                else
                {
                    int pmonth = month - 1;
                    data.AddInt32(pmonth); 
                    data.AddInt32(DateTime.Now.Year); 
                }
                data.AddInt32(rnd.Next(1,10));
                data.AddInt32(m_vScore / 2);
            }
            else
            {
                data.AddInt32(0); //1
                data.AddInt32(0); //2
                data.AddInt32(0); //3
                data.AddInt32(0); //4
                data.AddInt32(0); //5
                data.AddInt32(0); //6
                data.AddInt32(0); //7
                data.AddInt32(0); //8
                data.AddInt32(0); //9
                data.AddInt32(0); //10
                data.AddInt32(0); //11
            }

            data.AddInt32(m_vLeagueId);
            data.AddInt32(GetAllianceCastleLevel());
            data.AddInt32(GetAllianceCastleTotalCapacity());
            data.AddInt32(GetAllianceCastleUsedCapacity());
            data.AddInt32(0);
            data.AddInt32(-1);
            data.AddInt32(GetTownHallLevel());
            data.AddString(m_vAvatarName);
            data.AddInt32(-1);
            data.AddInt32(m_vAvatarLevel);
            data.AddInt32(m_vExperience);
            data.AddInt32(m_vCurrentGems);
            data.AddInt32(m_vCurrentGems);
            data.AddInt32(1200);
            data.AddInt32(60);
            data.AddInt32(m_vScore);
            data.AddInt32(200); // Attack Wins
            data.AddInt32(1);
            data.AddInt32(100); // Attack Loses
            data.AddInt32(0);

            data.AddInt32(m_vAlliance_Gold);
            data.AddInt32(m_vAlliance_Elixir); 
            data.AddInt32(m_vAlliance_DarkElixir); 
            data.AddInt32(0);
            data.Add(1);
            data.AddInt64(946720861000);

            data.Add(m_vnameChosenByUser);

            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(1);

            data.AddInt32(0);
            data.AddInt32(0);
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
            foreach (var u in AllianceUnits)
            {
                data.AddRange(BitConverter.GetBytes(u.Data.GetGlobalID()).Reverse());
                data.AddRange(BitConverter.GetBytes(u.Value).Reverse());
                data.AddRange(BitConverter.GetBytes(0).Reverse()); 
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

            data.AddInt32(QuickTrain1.Count); 
            foreach (var i in QuickTrain1)
            {
                data.AddInt32(i.Data.GetGlobalID());
                data.AddInt32(i.Value);
            }

            data.AddInt32(QuickTrain2.Count); 
            foreach (var i in QuickTrain2)
            {
                data.AddInt32(i.Data.GetGlobalID());
                data.AddInt32(i.Value);
            }
            data.AddInt32(QuickTrain3.Count); 
            foreach (var i in QuickTrain3)
            {
                data.AddInt32(i.Data.GetGlobalID());
                data.AddInt32(i.Value);
            }
            data.AddInt32(QuickTrain1.Count); 
            foreach (var i in QuickTrain1)
            {
                data.AddInt32(i.Data.GetGlobalID());
                data.AddInt32(i.Value);
            }
            data.AddDataSlots(new List<DataSlot>());
            return data.ToArray();
        }

        public long GetAllianceId() => m_vAllianceId;

        public AllianceMemberEntry GetAllianceMemberEntry()
        {
            var alliance = ObjectManager.GetAlliance(m_vAllianceId);
            if (alliance != null)
                return alliance.GetAllianceMember(m_vId);
            return null;
        }

        public DateTime GetAccountCreationDate() => m_vAccountCreationDate;

        public int GetAllianceRole()
        {
            var ame = GetAllianceMemberEntry();
            if (ame != null)
                return ame.GetRole();
            return -1;
        }

        public int GetAvatarHighIdInt() => m_vHighInt;

        public int GetAlliance_Gold() => m_vAlliance_Gold;

        public int GetAlliance_Elixir() => m_vAlliance_Elixir;

        public int GetAlliance_DarkElixir() => m_vAlliance_DarkElixir;

        public int GetAvataLowIdInt() => m_vLowInt;

        public int GetAvatarLevel() => m_vAvatarLevel;

        public int GetActiveLayout() =>  m_vActiveLayout;

        public string GetAvatarName() => m_vAvatarName;

        public long GetCurrentHomeId() => m_vCurrentHomeId;

        public int GetDiamonds() => m_vCurrentGems;

        public bool GetAndroid() => m_vAndroid;

        public bool GetPremium() => m_vPremium;

        public long GetId() => m_vId;

        public int GetShieldTime => m_vShieldTime;

        public int GetProtectionTime => m_vProtectionTime;

        public int GetLeagueId() => m_vLeagueId;

        public int GetScore()
        {
            updateLeague();
            return m_vScore;
        }

        public int GetSecondsFromLastUpdate() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - LastUpdate;

        public string GetUserToken() => m_vToken;

        public string GetUserRegion() => m_vRegion;

        public bool HasEnoughDiamonds(int diamondCount) => m_vCurrentGems >= diamondCount;

        public bool HasEnoughResources(ResourceData rd, int buildCost) => GetResourceCount(rd) >= buildCost;

        public void LoadFromJSON(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);
            m_vId = jsonObject["avatar_id"].ToObject<long>();
            m_vHighInt = jsonObject["id_high_int"].ToObject<int>();
            m_vLowInt = jsonObject["id_low_int"].ToObject<int>();
            m_vToken = jsonObject["token"].ToObject<string>();
            m_vRegion = jsonObject["region"].ToObject<string>();
            m_vAccountCreationDate = jsonObject["avatar_creation_date"].ToObject<DateTime>();
            m_vActiveLayout = jsonObject["active_layout"].ToObject<int>();
            m_vAndroid = jsonObject["android"].ToObject<bool>();
            m_vCurrentHomeId = jsonObject["current_home_id"].ToObject<long>();
            m_vAllianceId = jsonObject["alliance_id"].ToObject<long>();
            SetAllianceCastleLevel(jsonObject["alliance_castle_level"].ToObject<int>());
            SetAllianceCastleTotalCapacity(jsonObject["alliance_castle_total_capacity"].ToObject<int>());
            SetAllianceCastleUsedCapacity(jsonObject["alliance_castle_used_capacity"].ToObject<int>());
            SetTownHallLevel(jsonObject["townhall_level"].ToObject<int>());
            m_vAvatarName = jsonObject["avatar_name"].ToObject<string>();
            m_vAvatarLevel = jsonObject["avatar_level"].ToObject<int>();
            m_vExperience = jsonObject["experience"].ToObject<int>();
            m_vCurrentGems = jsonObject["current_gems"].ToObject<int>();
            SetScore(jsonObject["score"].ToObject<int>());
            m_vNameChangingLeft = jsonObject["nameChangesLeft"].ToObject<byte>();
            m_vnameChosenByUser = jsonObject["nameChosenByUser"].ToObject<byte>();
            m_vShieldTime = jsonObject["shield_time"].ToObject<int>();
            m_vProtectionTime = jsonObject["protection_time"].ToObject<int>();

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
                TroopDataSlot ds = new TroopDataSlot(null, 0, 0);
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
            JObject jsonData = new JObject();
            jsonData.Add("avatar_id", m_vId);
            jsonData.Add("id_high_int", m_vHighInt);
            jsonData.Add("id_low_int", m_vLowInt);
            jsonData.Add("token", m_vToken);
            jsonData.Add("region", m_vRegion);
            jsonData.Add("avatar_creation_date", m_vAccountCreationDate);
            jsonData.Add("active_layout", m_vActiveLayout);
            jsonData.Add("android", m_vAndroid);
            jsonData.Add("current_home_id", m_vCurrentHomeId);
            jsonData.Add("alliance_id", m_vAllianceId);
            jsonData.Add("alliance_castle_level", GetAllianceCastleLevel());
            jsonData.Add("alliance_castle_total_capacity", GetAllianceCastleTotalCapacity());
            jsonData.Add("alliance_castle_used_capacity", GetAllianceCastleUsedCapacity());
            jsonData.Add("townhall_level", GetTownHallLevel());
            jsonData.Add("avatar_name", m_vAvatarName);
            jsonData.Add("avatar_level", m_vAvatarLevel);
            jsonData.Add("experience", m_vExperience);
            jsonData.Add("current_gems", m_vCurrentGems);
            jsonData.Add("score", GetScore());
            jsonData.Add("nameChangesLeft", m_vNameChangingLeft);
            jsonData.Add("nameChosenByUser", (ushort) m_vnameChosenByUser);
            jsonData.Add("shield_time", m_vShieldTime);
            jsonData.Add("protection_time", m_vProtectionTime);

            JArray jsonBookmarkClan = new JArray();
            foreach (var clan in BookmarkedClan)
                jsonBookmarkClan.Add(clan.Save(new JObject()));
            jsonData.Add("bookmark", jsonBookmarkClan);

            JArray jsonResourcesArray = new JArray();
            foreach (var resource in GetResources())
                jsonResourcesArray.Add(resource.Save(new JObject()));
            jsonData.Add("resources", jsonResourcesArray);

            JArray jsonUnitsArray = new JArray();
            foreach (var unit in GetUnits())
                jsonUnitsArray.Add(unit.Save(new JObject()));
            jsonData.Add("units", jsonUnitsArray);

            JArray jsonSpellsArray = new JArray();
            foreach (var spell in GetSpells())
                jsonSpellsArray.Add(spell.Save(new JObject()));
            jsonData.Add("spells", jsonSpellsArray);

            JArray jsonUnitUpgradeLevelsArray = new JArray();
            foreach (var unitUpgradeLevel in m_vUnitUpgradeLevel)
                jsonUnitUpgradeLevelsArray.Add(unitUpgradeLevel.Save(new JObject()));
            jsonData.Add("unit_upgrade_levels", jsonUnitUpgradeLevelsArray);

            JArray jsonSpellUpgradeLevelsArray = new JArray();
            foreach (var spellUpgradeLevel in m_vSpellUpgradeLevel)
                jsonSpellUpgradeLevelsArray.Add(spellUpgradeLevel.Save(new JObject()));
            jsonData.Add("spell_upgrade_levels", jsonSpellUpgradeLevelsArray);

            JArray jsonHeroUpgradeLevelsArray = new JArray();
            foreach (var heroUpgradeLevel in m_vHeroUpgradeLevel)
                jsonHeroUpgradeLevelsArray.Add(heroUpgradeLevel.Save(new JObject()));
            jsonData.Add("hero_upgrade_levels", jsonHeroUpgradeLevelsArray);

            JArray jsonHeroHealthArray = new JArray();
            foreach (var heroHealth in m_vHeroHealth)
                jsonHeroHealthArray.Add(heroHealth.Save(new JObject()));
            jsonData.Add("hero_health", jsonHeroHealthArray);

            JArray jsonHeroStateArray = new JArray();
            foreach (var heroState in m_vHeroState)
                jsonHeroStateArray.Add(heroState.Save(new JObject()));
            jsonData.Add("hero_state", jsonHeroStateArray);

            JArray jsonAllianceUnitsArray = new JArray();
            foreach (var allianceUnit in AllianceUnits)
                jsonAllianceUnitsArray.Add(allianceUnit.Save(new JObject()));
            jsonData.Add("alliance_units", jsonAllianceUnitsArray);

            jsonData.Add("tutorial_step", TutorialStepsCount);

            JArray jsonAchievementsProgressArray = new JArray();
            foreach (var achievement in Achievements)
                jsonAchievementsProgressArray.Add(achievement.Save(new JObject()));
            jsonData.Add("achievements_progress", jsonAchievementsProgressArray);

            JArray jsonNpcStarsArray = new JArray();
            foreach (var npcLevel in NpcStars)
                jsonNpcStarsArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_stars", jsonNpcStarsArray);

            JArray jsonNpcLootedGoldArray = new JArray();
            foreach (var npcLevel in NpcLootedGold)
                jsonNpcLootedGoldArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_looted_gold", jsonNpcLootedGoldArray);

            JArray jsonNpcLootedElixirArray = new JArray();
            foreach (var npcLevel in NpcLootedElixir)
                jsonNpcLootedElixirArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_looted_elixir", jsonNpcLootedElixirArray);

            JArray jsonQuickTrain1Array = new JArray();
            foreach (var quicktrain1 in QuickTrain1)
                jsonQuickTrain1Array.Add(quicktrain1.Save(new JObject()));
            jsonData.Add("quick_train_1", jsonQuickTrain1Array);

            JArray jsonQuickTrain2Array = new JArray();
            foreach (var quicktrain2 in QuickTrain2)
                jsonQuickTrain1Array.Add(quicktrain2.Save(new JObject()));
            jsonData.Add("quick_train_2", jsonQuickTrain2Array);

            JArray jsonQuickTrain3Array = new JArray();
            foreach (var quicktrain3 in QuickTrain3)
                jsonQuickTrain3Array.Add(quicktrain3.Save(new JObject()));
            jsonData.Add("quick_train_3", jsonQuickTrain3Array);

            jsonData.Add("Premium", m_vPremium);

            return JsonConvert.SerializeObject(jsonData);
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

        public void SetAllianceId(long id) => m_vAllianceId = id;

        public void SetActiveLayout(int layout) => m_vActiveLayout = layout;

        public void SetAndroid(bool android) => m_vAndroid = android;

        public void SetAllianceRole(int a)
        {
            AllianceMemberEntry ame = GetAllianceMemberEntry();
            if (ame != null)
                ame.SetRole(a);
        }

        public void SetDiamonds(int count) => m_vCurrentGems = count;

        public void SetLeagueId(int id) => m_vLeagueId = id;

        public void SetName(string name)
        {
            m_vAvatarName = name;
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

        public void SetToken(string token) => m_vToken = token;

        public void SetRegion(string region) => m_vRegion = region;

        public void SetAvatarLevel(int newlv) => m_vAvatarLevel = newlv;

        public void UseDiamonds(int diamondCount) => m_vCurrentGems -= diamondCount;
    }
}
