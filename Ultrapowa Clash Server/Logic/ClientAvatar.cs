namespace UCS.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using UCS.Files.Logic;
    using UCS.Helpers;
    using UCS.Core;
    using UCS.Helpers.List;
    using UCS.Logic.JSONProperty;
    using DonationSlot = UCS.Logic.JSONProperty.DonationSlot;
    using Resources = UCS.Logic.JSONProperty.Resources;
    using UCS.Logic.API;
    using UCS.Logic.Enums;

    internal class ClientAvatar
    {
        #region JSONProperty
     
        [JsonProperty("acc_hi")] internal long HighID;
        [JsonProperty("acc_lo")] internal long LowID;

        internal long UserID
        {
            get { return (((long) this.HighID << 32) | (this.LowID & 0xFFFFFFFFL)); }
            set
            {
                this.HighID = value >> 32;
                this.LowID = (int) value;
            }
        }

        [JsonProperty("alli_hi")] internal long ClanHighID;
        [JsonProperty("alli_lo")] internal long ClanLowID;

        internal long AllianceID
        {
            get { return (((long) this.ClanHighID << 32) | (this.ClanLowID & 0xFFFFFFFFL)); }
            set
            {
                this.ClanHighID = value >> 32;
                this.ClanLowID = (int) value;
            }
        }

        [JsonProperty("token")] internal string Token;
        [JsonProperty("password")] internal string Password;

        [JsonProperty("name")] internal string Username = "NoNameYet";
        [JsonProperty("IPAddress")] internal string IPAddress;
        [JsonProperty("region")] internal string Region;

        /* INFORMATIONS */

        [JsonProperty("lvl")] internal int Level = 1;
        [JsonProperty("xp")] internal int Experience;

        [JsonProperty("wins")] internal int Wins;
        [JsonProperty("loses")] internal int Loses;
        [JsonProperty("games")] internal int Games;
        [JsonProperty("win_streak")] internal int Streak;
        [JsonProperty("donations")] internal int Donations;
        [JsonProperty("received")] internal int Received;

        [JsonProperty("shield")] internal int Shield;
        [JsonProperty("guard")] internal int Guard;
        [JsonProperty("legend_troph")] internal int Legendary_Trophies = 0;

        [JsonProperty("tutorial")] internal int Tutorial = 10;
        [JsonProperty("name_state")]   internal byte NameState = 0;

        [JsonProperty("reported")] internal int ReportedCount;
        [JsonProperty("got_reported")] internal int GotReported;

        [JsonProperty("layout")] internal int ActiveLayout = 0;
        //[JsonProperty("rank")] internal Rank Rank = Rank.Player;
        [JsonProperty("trophies")] internal int Trophies = Utils.ParseConfigString("startingTrophies") == "random" ? Core.Resources.Random.Next(1500, 4999) : Utils.ParseConfigInt("startingTrophies");
        [JsonProperty("league_type")]
        internal int League = 0;

        [JsonProperty("war_state")]
        internal bool WarState = true;
        [JsonProperty("premium")]
        internal bool Premium = false;
        [JsonProperty("account_priv")] internal byte AccountPrivileges = 0;

        [JsonProperty("castle_lvl")] internal int Castle_Level = -1;
        [JsonProperty("castle_total")] internal int Castle_Total = 0;
        [JsonProperty("castle_used")] internal int Castle_Used = 0;
        [JsonProperty("castle_total_sp")] internal int Castle_Total_SP = 0;
        [JsonProperty("castle_used_sp")] internal int Castle_Used_SP = 0;
        [JsonProperty("castle_gold")] internal int Castle_Gold = 0;
        [JsonProperty("castle_elixir")] internal int Castle_Elixir = 0;
        [JsonProperty("castle_dark_elixir")] internal int Castle_Dark_Elixir = 0;
        [JsonProperty("town_hall_lvl")]  internal int TownHall_Level = 0;

        [JsonProperty("resources")]  internal Resources Resources;
        [JsonProperty("resources_cap")] internal Resources Resources_Cap;

        [JsonProperty("units")] internal Units Units;
        [JsonProperty("spells")] internal Units Spells;
        [JsonProperty("alliance_units")] internal List<DonationSlot> Castle_Units;
        [JsonProperty("alliance_spells")] internal List<DonationSlot> Castle_Spells;


        [JsonProperty("units_level")] internal Upgrades UnitUpgradeLevel;
        [JsonProperty("spells_level")] internal Upgrades SpellsUpgradeLevel;

        [JsonProperty("hero_health")] internal List<Slot> HeroHealth;
        [JsonProperty("hero_level")] internal Upgrades HeroUpgradeLevel;
        [JsonProperty("hero_state")] internal List<Slot> HeroState;

        [JsonProperty("npcs")] internal Npcs Npcs;
        [JsonProperty("variable")] internal List<Slot> Variables;
        [JsonProperty("last_search_enemy_id")] internal List<long> LastAttackEnemyId = new List<long>();
        [JsonProperty("stream")] internal List<long[]> Stream = new List<long[]>();
        [JsonProperty("bookmark")] internal List<long> Bookmark = new List<long>();

        [JsonProperty("update_date")] internal DateTime Update = DateTime.UtcNow;
        [JsonProperty("creation_date")] internal DateTime Created = DateTime.UtcNow;
        [JsonProperty("ban_date")] internal DateTime BanTime = DateTime.UtcNow;

        [JsonProperty("achievements")] internal Achievements Achievements;
        [JsonProperty("facebook")] internal API.Facebook Facebook;
        [JsonProperty("google")] internal API.Google Google;
        [JsonProperty("gamecenter")] internal Gamecenter Gamecenter;
        [JsonProperty("inbox")] internal Inbox Inbox;

        [JsonProperty("qtrain1")] internal List<Slot> QuickTrain1;
        [JsonProperty("qtrain2")] internal List<Slot> QuickTrain2;
        [JsonProperty("qtrain3")] internal List<Slot> QuickTrain3;


        internal bool Banned => this.BanTime > DateTime.UtcNow;
        internal long BattleId = 0;
        #endregion

        public ClientAvatar()
        {
            this.Achievements = new Achievements();
            this.Facebook = new Facebook(this);
            this.Google = new Google(this);
            this.Gamecenter = new Gamecenter(this);
            this.Inbox = new Inbox(this);
            this.Resources = new Resources(this);

            this.Npcs = new Npcs();
            this.Variables = new List<Slot>();
            this.Castle_Units = new List<DonationSlot>();
            this.Castle_Spells = new List<DonationSlot>();
            this.Units = new Units(this);
            this.Spells = new Units(this);

            this.UnitUpgradeLevel = new Upgrades(this);
            this.SpellsUpgradeLevel = new Upgrades(this);
            this.HeroUpgradeLevel = new Upgrades(this);


            this.HeroState = new List<Slot>();
            this.HeroHealth = new List<Slot>();
            this.QuickTrain1 = new List<Slot>();
            this.QuickTrain2 = new List<Slot>();
            this.QuickTrain3 = new List<Slot>();
        }

        public ClientAvatar(long id)
        {
            this.UserID = id;

            this.Achievements = new Achievements();
            this.Facebook = new Facebook(this);
            this.Google = new Google(this);
            this.Gamecenter = new Gamecenter(this);
            this.Inbox = new Inbox(this);
            this.Resources = new Resources(this, true);       
            this.Resources_Cap = new Resources(this, true);

            this.Npcs = new Npcs();
            this.Variables = new List<Slot>();
            this.Castle_Units = new List<DonationSlot>();
            this.Castle_Spells = new List<DonationSlot>();
            this.Units = new Units(this);
            this.Spells = new Units(this);

            this.SpellsUpgradeLevel = new Upgrades(this);
            this.HeroUpgradeLevel = new Upgrades(this);
            this.UnitUpgradeLevel = new Upgrades(this);

            this.HeroState = new List<Slot>();
            this.HeroHealth = new List<Slot>();
            this.QuickTrain1 = new List<Slot>();
            this.QuickTrain2 = new List<Slot>();
            this.QuickTrain3 = new List<Slot>();

            this.GetTrophies();
        }

        public async Task<byte[]> Encode()
        {
            try
            {
                List<byte> data = new List<byte>();
                data.AddLong(this.UserID);
                data.AddLong(this.UserID);
                if (this.AllianceID != 0)
                {
                    data.Add(1);
                    data.AddLong(this.AllianceID);
                    Alliance alliance = await ObjectManager.GetAlliance(this.AllianceID);
                    data.AddString(alliance.GetAllianceName());
                    data.AddInt(alliance.GetAllianceBadgeData());
                    data.AddInt(alliance.GetAllianceMember(this.UserID).GetRole());
                    data.AddInt(alliance.GetAllianceLevel());
                }
                data.Add(0);

                if (this.League == 22)
                {
                    data.AddInt(this.Trophies / 12);
                    data.AddInt(1);
                    int month = DateTime.Now.Month;
                    data.AddInt(month);
                    data.AddInt(DateTime.Now.Year);
                    data.AddInt(Core.Resources.Random.Next(1, 10));
                    data.AddInt(this.Trophies);
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
                    data.AddInt(Core.Resources.Random.Next(1, 10));
                    data.AddInt(this.Trophies / 2);
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

                data.AddInt(this.League);
                data.AddInt(this.Castle_Level);
                data.AddInt(this.Castle_Total);
                data.AddInt(this.Castle_Used);
                data.AddInt(this.Castle_Total_SP);
                data.AddInt(this.Castle_Used_SP);
                data.AddInt(this.TownHall_Level);
#if DEBUG
                data.AddString(this.Username + " #" + this.UserID);
#else
                data.AddString(this.Username);
#endif
                data.AddString(this.Facebook.Identifier);
                data.AddInt(this.Level);
                data.AddInt(this.Experience);
                data.AddInt(this.Resources.Gems);
                data.AddInt(this.Resources.Gems);
                data.AddInt(1200);
                data.AddInt(60);

                data.AddInt(this.Trophies);
                data.AddInt(this.Wins);
                data.AddInt(this.Loses);
                data.AddInt(0); // Attack Loses
                data.AddInt(0);

                data.AddInt(this.Castle_Gold);
                data.AddInt(this.Castle_Elixir);
                data.AddInt(this.Castle_Dark_Elixir);
                data.AddInt(0);
                data.Add(1);
                data.AddLong(946720861000);

                data.AddBool(this.NameState > 0);

                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(this.WarState ? 1 : 0);

                data.AddInt(0);
                data.AddInt(0);
                data.AddBool(this.NameState > 1);

                data.AddRange(this.Resources_Cap.ToBytes);
                data.AddRange(this.Resources.ToBytes);

                data.AddDataSlots(this.Units);
                data.AddDataSlots(this.Spells);
                data.AddDataSlots(this.UnitUpgradeLevel);
                data.AddDataSlots(this.SpellsUpgradeLevel);

                data.AddDataSlots(this.HeroUpgradeLevel);
                data.AddDataSlots(this.HeroHealth);
                data.AddDataSlots(this.HeroState);

                data.AddInt(this.Castle_Units.Count + this.Castle_Spells.Count);

                foreach (DonationSlot u in this.Castle_Units)
                {
                    data.AddInt(u.Data);
                    data.AddInt(u.Count);
                    data.AddInt(u.Level);
                }

                foreach (DonationSlot u in this.Castle_Spells)
                {
                    data.AddInt(u.Data);
                    data.AddInt(u.Count);
                    data.AddInt(u.Level);
                }

                data.AddInt(this.Tutorial);
                for (int i = 0; i < this.Tutorial; i++)
                    data.AddInt(21000000 + i);

                data.AddInt(this.Achievements.Completed.Count);
                foreach (Slot Achievement in this.Achievements.Completed)
                {
                    data.AddInt(Achievement.Data);
                }

                data.AddInt(this.Achievements.Completed.Count);
                foreach (Slot Achievement in this.Achievements.Completed)
                {
                    data.AddInt(Achievement.Data);
                    data.AddInt(Achievements.Count);
                }


                data.AddRange(this.Npcs.ToBytes);

                data.AddInt(this.Variables.Count);
                foreach (Slot Variable in this.Variables)
                {
                    data.AddInt(Variable.Data);
                    data.AddInt(Variable.Count);
                }
                data.AddDataSlots(new List<Slot>());
                data.AddDataSlots(new List<Slot>());
                data.AddDataSlots(new List<Slot>());

                data.AddDataSlots(QuickTrain1);
                data.AddDataSlots(QuickTrain2);
                data.AddDataSlots(QuickTrain3);
                data.AddDataSlots(QuickTrain1);
                return data.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #region Set

        void updateLeague()
        {
            var table = CSVManager.DataTables.GetTable(12);
            int i = 0;
            bool found = false;
            while (!found)
            {
                var league = (LeagueData) table.GetItemAt(i);
                if (Trophies <= league.BucketPlacementRangeHigh[league.BucketPlacementRangeHigh.Count - 1] &&
                    Trophies >= league.BucketPlacementRangeLow[0])
                {
                    found = true;
                    SetLeagueId(i);
                }
                i++;
            }
        }


        public void AddDiamonds(int diamondCount) => this.Resources.Plus(Resource.Diamonds, diamondCount);

        public void AddExperience(int exp)
        {
            this.Experience += exp;
            var experienceCap = ((ExperienceLevelData) CSVManager.DataTables.GetTable(10).GetDataByName(this.Level.ToString())).ExpPoints;
            if (this.Experience >= experienceCap)
                if (CSVManager.DataTables.GetTable(10).GetItemCount() > this.Level + 1)
                {
                    this.Level += 1;
                    this.Experience = this.Experience - experienceCap;
                }
                else
                    this.Experience = 0;
        }

        #endregion

        #region  Get 

        public async Task<AllianceMemberEntry> GetAllianceMemberEntry()
        {
            try
            {
                var alliance = await ObjectManager.GetAlliance(this.AllianceID);
                return alliance?.GetAllianceMember(this.UserID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> GetAllianceRole()
        {
            try
            {
                var ame = await GetAllianceMemberEntry();
                if (ame != null)
                    return ame.GetRole();
                return -1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public int GetTrophies()
        {
            updateLeague();
            return this.Trophies;
        }


        public bool HasEnoughDiamonds(int diamondCount) => this.Resources.Gems  >= diamondCount;

        public bool HasEnoughResources(ResourceData rd, int buildCost) => GetResourceCount(rd) >= buildCost;

        #endregion

        #region Meth
        public static int GetDataIndex(Upgrades dsl, Data d) => dsl.FindIndex(ds => ds.Data == d.GetGlobalID());
        public static int GetDataIndex(List<Slot> dsl, Data d) => dsl.FindIndex(ds => ds.Data == d.GetGlobalID());
        public static int GetDataIndex(Units dsl, Data d) => dsl.FindIndex(ds => ds.Data == d.GetGlobalID());
        public static int GetDataIndex(Resources dsl, Data d) => dsl.FindIndex(ds => ds.Data == d.GetGlobalID());

        public int GetResourceCount(ResourceData rd)
        {
            int index = GetDataIndex(this.Resources, rd);
            int count = 0;
            if (index != -1)
            {
                count = this.Resources[index].Count;
            }
            return count;
        }

        public int GetResourceCap(ResourceData rd)
        {
            int index = GetDataIndex(this.Resources_Cap, rd);
            int count = 0;
            if (index != -1)
            {
                count = this.Resources_Cap[index].Count;
            }
            return count;
        }

        public int GetUnitCount(CombatItemData cd)
        {
            int result = 0;
            if (cd.GetCombatItemType() == 1)
            {
                int index = GetDataIndex(this.Spells, cd);
                if (index != -1)
                {
                    result = this.Spells[index].Count;
                }
            }
            else
            {
                int index = GetDataIndex(this.Units, cd);
                if (index != -1)
                {
                    result = this.Units[index].Count;
                }
            }
            return result;
        }
        public int GetUnusedResourceCap(ResourceData rd)
        {
            int resourceCount = GetResourceCount(rd);
            int resourceCap = GetResourceCap(rd);
            return Math.Max(resourceCap - resourceCount, 0);
        }

        public void SetUnitUpgradeLevel(CombatItemData cd, int level)
        {
            switch (cd.GetCombatItemType())
            {
                case 2:
                    {
                        int index = GetDataIndex(this.HeroUpgradeLevel, cd);
                        if (index != -1)
                        {
                            this.HeroUpgradeLevel[index].Data = level;
                        }
                        else
                        {
                            Slot ds = new Slot(cd.GetGlobalID(), level);
                            this.HeroUpgradeLevel.Add(ds);
                        }
                        break;
                    }
                case 1:
                    {
                        int index = GetDataIndex(this.SpellsUpgradeLevel, cd);
                        if (index != -1)
                        {
                            this.SpellsUpgradeLevel[index].Data = level;
                        }
                        else
                        {
                            Slot ds = new Slot(cd.GetGlobalID(), level);
                            this.SpellsUpgradeLevel.Add(ds);
                        }
                        break;
                    }
                default:
                    {
                        int index = GetDataIndex(this.UnitUpgradeLevel, cd);
                        if (index != -1)
                        {
                            this.UnitUpgradeLevel[index].Data = level;
                        }
                        else
                        {
                            Slot ds = new Slot(cd.GetGlobalID(), level);
                            this.UnitUpgradeLevel.Add(ds);
                        }
                        break;
                    }
            }
        }
        public int GetUnitUpgradeLevel(CombatItemData cd)
        {
            int result = 0;
            switch (cd.GetCombatItemType())
            {
                case 2:
                    {
                        int index = GetDataIndex(this.HeroUpgradeLevel, cd);
                        if (index != -1)
                        {
                            result = this.HeroUpgradeLevel[index].Data;
                        }
                        break;
                    }
                case 1:
                    {
                        int index = GetDataIndex(this.SpellsUpgradeLevel, cd);
                        if (index != -1)
                        {
                            result = this.SpellsUpgradeLevel[index].Data;
                        }
                        break;
                    }

                default:
                    {
                        int index = GetDataIndex(this.UnitUpgradeLevel, cd);
                        if (index != -1)
                        {
                            result = this.UnitUpgradeLevel[index].Data;
                        }
                        break;
                    }
            }
            return result;
        }
        public void SetUnitCount(CombatItemData cd, int count)
        {
            switch (cd.GetCombatItemType())
            {
                case 1:
                    {
                        int index = GetDataIndex(this.Spells, cd);
                        if (index != -1)
                        {
                            this.Spells[index].Data = count;
                        }
                        else
                        {
                            Slot ds = new Slot(cd.GetGlobalID(), count);
                            this.Spells.Add(ds);
                        }
                        break;
                    }
                default:
                    {
                        int index = GetDataIndex(this.Units, cd);
                        if (index != -1)
                        {
                            this.Units[index].Data = count;
                        }
                        else
                        {
                            Slot ds = new Slot(cd.GetGlobalID(), count);
                            this.Units.Add(ds);
                        }
                        break;
                    }

            }

        }
        public void CommodityCountChangeHelper(int commodityType, Data data, int count)
        {
            if (data.GetDataType() == 2)
            {
                if (commodityType == 0)
                {
                    int resourceCount = this.Resources.Get(data.GetGlobalID());
                    int newResourceValue = Math.Max(resourceCount + count, 0);
                    if (count >= 1)
                    {
                        int resourceCap = GetResourceCap((ResourceData)data);
                        if (resourceCount < resourceCap)
                        {
                            if (newResourceValue > resourceCap)
                            {
                                newResourceValue = resourceCap;
                            }
                        }
                    }
                    this.Resources.Set(data.GetGlobalID(), newResourceValue);
                }
            }
        }

        public void SetHeroHealth(HeroData hd, int health)
        {
            int index = GetDataIndex(this.HeroHealth, hd);
            if (index == -1)
            {
                Slot ds = new Slot(hd.GetGlobalID(), health);
                this.HeroHealth.Add(ds);
            }
            else
            {
                this.HeroHealth[index].Count = health;
            }
        }

        public void SetHeroState(HeroData hd, int state)
        {
            int index = GetDataIndex(this.HeroState, hd);
            if (index == -1)
            {
                Slot ds = new Slot(hd.GetGlobalID(), state);
                this.HeroState.Add(ds);
            }
            else
            {
                this.HeroState[index].Count = state;
            }
        }


        #endregion

        #region Method


        public void Add_Castle_Unit(long Id, int Data, int Count, int level)
        {
            int _Index = this.Castle_Units.FindIndex(U => U.DonatorID == Id && U.Data == Data && U.Level == level);

            if (_Index > -1)
            {
                this.Units[_Index].Count += Count;
            }
            else
            {
                this.Castle_Units.Add(new DonationSlot(Id, Data, Count, level));
            }
        }

        public async void SetAllianceRole(int a)
        {
            try
            {
                AllianceMemberEntry ame = await GetAllianceMemberEntry();
                ame?.SetRole(a);
            }
            catch (Exception)
            {
            }
        }

        public void SetDiamonds(int count) => this.Resources.Set(Resource.Diamonds, count);

        public void SetLeagueId(int id) => this.League = id;

        public void SetName(string name)
        {
            this.Username = name;
            this.NameState += 1;
            this.Tutorial += 3;
        }

        public void SetTrophies(int newScore)
        {
            this.Trophies = newScore;
            updateLeague();
        }

        public void SetAvatarLevel(int newlv) => this.Level = newlv;

        public void UseDiamonds(int diamondCount) => this.Resources.Minus(Resource.Diamonds, diamondCount);

        #endregion
    }
}