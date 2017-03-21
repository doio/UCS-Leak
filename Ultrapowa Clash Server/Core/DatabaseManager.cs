using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using MySql.Data.MySqlClient;
using System.Linq;
using UCS.Database;
using UCS.Logic;
using UCS.Core.Settings;
using static UCS.Core.Logger;
using System.Threading.Tasks;
using UCS.Logic.Enums;
using UCS.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Logic.JSONProperty;

namespace UCS.Core
{
    internal class DatabaseManager
    {

        internal JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        private string Mysql;

        public DatabaseManager()
        {

        }

        public static DatabaseManager Single() => new DatabaseManager();

        public void CreateAccount(Level l)
        {
            try
            {
                string Object = JsonConvert.SerializeObject(l.Avatar, this._settings);
                if (Constants.UseCacheServer)
                {
                    Redis.Players.StringSet(l.Avatar.UserID.ToString(), Object + "#:#:#:#" + l.SaveToJSON(), TimeSpan.FromHours(4));
                }

                using (Mysql db = new Mysql())
                {
                    db.Player.Add(new Player
                    {
                        PlayerId = l.Avatar.UserID,
                        Avatar = Object,
                        GameObjects = l.SaveToJSON()
                    }
                    );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CreateBattle(Battle b)
        {
            try
            {
                string BattleData = JsonConvert.SerializeObject(b, this._settings);
                string ReplayInfo = JsonConvert.SerializeObject(b.Replay_Info, this._settings);
                if (Constants.UseCacheServer)
                {
                    Redis.Stream.StringSet(b.Battle_ID.ToString(), BattleData + "#:#:#:#" + ReplayInfo, TimeSpan.FromHours(4));
                }

                using (Mysql db = new Mysql())
                {
                    db.Stream.Add(new Stream
                    {
                        StreamId = b.Battle_ID,
                        ReplayData = ReplayInfo,
                        BattleData = BattleData
                    }
                    );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void CreateAlliance(Alliance a)
        {
            try
            {
                if (Constants.UseCacheServer) //Redis As Cache Server
                    Redis.Clans.StringSet(a.AllianceID.ToString(), a.SaveToJSON(), TimeSpan.FromHours(4));

                using (Mysql db = new Mysql())
                {
                    db.Clan.Add(
                        new Clan()
                        {
                            ClanId = a.AllianceID,
                            LastUpdateTime = DateTime.Now,
                            Data = a.SaveToJSON()
                        }
                    );
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public Battle GetBattle(long BattleId)
        {
            Battle _Battle = new Battle(BattleId, new Level(), new Level() , false);
            if (Constants.UseCacheServer) //Redis As Cache Server
            {
                string _Data = Redis.Stream.StringGet(BattleId.ToString()).ToString();

                if (!string.IsNullOrEmpty(_Data) && _Data.Contains("#:#:#:#"))
                {
                    string[] _Datas = _Data.Split(new string[1] { "#:#:#:#" }, StringSplitOptions.None);

                    if (!string.IsNullOrEmpty(_Datas[0]) && !string.IsNullOrEmpty(_Datas[1]))
                    {
                        _Battle = JsonConvert.DeserializeObject<Battle>(_Datas[0], _settings);
                        _Battle.Replay_Info = JsonConvert.DeserializeObject<Replay_Info>(_Datas[1], _settings);
                    }
                    else
                    {
                        _Battle = null;
                    }
                }
            }
            else
            {
                using (Mysql db = new Mysql())
                {
                    Stream p = db.Stream.Find(BattleId);

                    if (p != null)
                    {
                        _Battle = JsonConvert.DeserializeObject<Battle>(p.BattleData, _settings);
                        _Battle.Replay_Info = JsonConvert.DeserializeObject<Replay_Info>(p.ReplayData, _settings);
                    }
                };
            }
            return _Battle;
        }

        public async Task<Level> GetAccount(long playerId)
        {
            try
            {
                Level account = null;
                if (Constants.UseCacheServer) //Redis as cache server
                {
                    string _Data = Redis.Players.StringGet(playerId.ToString()).ToString();

                    if (!string.IsNullOrEmpty(_Data) && _Data.Contains("#:#:#:#"))
                    {
                        string[] _Datas = _Data.Split(new string[1] { "#:#:#:#" }, StringSplitOptions.None);

                        if (!string.IsNullOrEmpty(_Datas[0]) && !string.IsNullOrEmpty(_Datas[1]))
                        {
                            account = new Level
                            {
                                Avatar = JsonConvert.DeserializeObject<ClientAvatar>(_Datas[0], this._settings)
                            };
                            account.LoadFromJSON(_Datas[1]);
                        }
                    }
                    else
                    {
                        using (Mysql db = new Mysql())
                        {
                            Player p = await db.Player.FindAsync(playerId);

                            if (p != null)
                            {
                                account = new Level
                                {
                                    Avatar = JsonConvert.DeserializeObject<ClientAvatar>(p.Avatar, this._settings)
                                };
                                account.LoadFromJSON(p.GameObjects);
                                Redis.Players.StringSet(playerId.ToString(), p.Avatar + "#:#:#:#" + p.GameObjects,
                                    TimeSpan.FromHours(4));
                            }
                        };
                    }
                }
                else
                {
                    using (Mysql db = new Mysql())
                    {
                        Player p = await db.Player.FindAsync(playerId);

                        if (p != null)
                        {
                            account = new Level
                            {
                                Avatar = JsonConvert.DeserializeObject<ClientAvatar>(p.Avatar, this._settings)
                            };
                            account.LoadFromJSON(p.GameObjects);
                        }
                    }
                }
                return account;
            }
            catch (Exception message)
            {
                Console.WriteLine(message.Message);
                return null;
            }
        }

        public async Task<Alliance> GetAlliance(long allianceId)
        {
            try
            {
                Alliance alliance = null;
                if (Constants.UseCacheServer)
                {
                    string _Data = Redis.Clans.StringGet(allianceId.ToString()).ToString();


                    if (!string.IsNullOrEmpty(_Data))
                    {
                        alliance = new Alliance();
                        alliance.LoadFromJSON(_Data);
                    }
                    else
                    {
                        using (Mysql db = new Mysql())
                        {
                            Clan p = await db.Clan.FindAsync(allianceId);
                            if (p != null)
                            {
                                alliance = new Alliance();
                                alliance.LoadFromJSON(p.Data);
                                Redis.Clans.StringSet(allianceId.ToString(), p.Data, TimeSpan.FromHours(4));
                            }
                        }
                    }
                }
                else
                {
                    using (Mysql db = new Mysql())
                    {
                        Clan p = await db.Clan.FindAsync(allianceId);
                        if (p != null)
                        {
                            alliance = new Alliance();
                            alliance.LoadFromJSON(p.Data);
                        }
                    }
                }
                return alliance;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<long> GetAllPlayerIds()
        {
            List<long> ids = new List<long>();
            using (Mysql db = new Mysql())
                ids.AddRange(db.Player.Select(p => p.PlayerId));
            return ids;
        }

        public List<long> GetAllClanIds()
        {
            List<long> ids = new List<long>();
            using (Mysql db = new Mysql())
                ids.AddRange(db.Clan.Select(p => p.ClanId));
            return ids;
        }
        internal int GetClanSeed()
        {
            const string SQL = "SELECT coalesce(MAX(ClanId), 0) FROM Clan";
            int Seed = -1;

            using (MySqlConnection Conn = new MySqlConnection(this.Mysql))
            {
                Conn.Open();

                using (MySqlCommand CMD = new MySqlCommand(SQL, Conn))
                {
                    CMD.Prepare();
                    Seed = Convert.ToInt32(CMD.ExecuteScalar());
                }
            }

            return Seed;
        }
        internal int GetStreamSeed()
        {
            const string SQL = "SELECT coalesce(MAX(StreamId), 0) FROM Stream";
            int Seed = -1;

            using (MySqlConnection Conn = new MySqlConnection(this.Mysql))
            {
                Conn.Open();

                using (MySqlCommand CMD = new MySqlCommand(SQL, Conn))
                {
                    CMD.Prepare();
                    Seed = Convert.ToInt32(CMD.ExecuteScalar());
                }
            }

            return Seed;
        }

        public int GetPlayerSeed()
        {
            try
            {
                const string SQL = "SELECT coalesce(MAX(PlayerId), 0) FROM Player";
                int Seed = -1;

                var builder = new MySqlConnectionStringBuilder()
                {
                    Server = Utils.ParseConfigString("MysqlIPAddress"),
                    UserID = Utils.ParseConfigString("MysqlUsername"),
                    Port = (uint)Utils.ParseConfigInt("MysqlPort"),
                    Pooling = false,
                    Database = Utils.ParseConfigString("MysqlDatabase"),
                    MinimumPoolSize = 1
                };
                if (!string.IsNullOrWhiteSpace(Utils.ParseConfigString("MysqlPassword")))
                    builder.Password = Utils.ParseConfigString("MysqlPassword");
                Mysql = builder.ToString();

                using (MySqlConnection Conn = new MySqlConnection(Mysql))
                {
                    Conn.Open();

                    using (MySqlCommand CMD = new MySqlCommand(SQL, Conn))
                    {
                        CMD.Prepare();
                        Seed = Convert.ToInt32(CMD.ExecuteScalar());
                    }
                }

                return Seed;
            }
            catch (Exception ex)
            {
                Say();
                Error("An exception occured when reconnecting to the MySQL Server.");
                Error("Please check your database configuration!");
                Error(ex.Message);
                Console.ReadKey();
                UCSControl.UCSRestart();
            }
            return 0;
        }


        public void RemoveAlliance(Alliance alliance)
        {
            try
            {
                long id = alliance.AllianceID;
                using (Mysql db = new Mysql())
                {
                    db.Clan.Remove(db.Clan.Find((int)id));
                    db.SaveChanges();
                }
                ObjectManager.RemoveInMemoryAlliance(id);
            }
            catch (Exception)
            {
            }
        }

        public Level GetPlayerViaFacebook(string FacebookID)
        {
            try
            {
                Level account = null;
                Player Data = null;
                using (Mysql Database = new Mysql())
                {
                    Parallel.ForEach(Database.Player.ToList(), (Query, state) =>
                    {
                        if (Query.Avatar.Contains(FacebookID))
                        {
                            Data = Query;
                            state.Break();
                        }
                    });

                    if (Data != null)
                    {
                        account = new Level
                        {
                            Avatar = JsonConvert.DeserializeObject<ClientAvatar>(Data.Avatar, this._settings)
                        };
                        account.LoadFromJSON(Data.GameObjects);
                        if (Constants.UseCacheServer)
                            Redis.Players.StringSet(Data.PlayerId.ToString(), Data.Avatar + "#:#:#:#" + Data.GameObjects, TimeSpan.FromHours(4));
                    }

                }
                return account;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task Save(Alliance alliance)
        {
            try
            {
                if (Constants.UseCacheServer)
                    Redis.Clans.StringSet(alliance.AllianceID.ToString(), alliance.SaveToJSON(), TimeSpan.FromHours(4));

                using (Mysql context = new Mysql())
                {
                    Clan c = await context.Clan.FindAsync((int)alliance.AllianceID);
                    if (c != null)
                    {
                        c.LastUpdateTime = DateTime.Now;
                        c.Data = alliance.SaveToJSON();
                        context.Entry(c).State = EntityState.Modified;
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }
        }


        public void Save(Battle _Battle)
        {
            try
            {
                string BattleData = JsonConvert.SerializeObject(_Battle, this._settings);
                string ReplayInfo = JsonConvert.SerializeObject(_Battle.Replay_Info, this._settings);
                if (Constants.UseCacheServer)
                    Redis.Stream.StringSet(_Battle.Battle_ID.ToString(), BattleData + "#:#:#:#" + ReplayInfo,
                        TimeSpan.FromHours(4));

                using (Mysql context = new Mysql())
                {
                    Stream p = context.Stream.Find(_Battle.Battle_ID);
                    if (p != null)
                    {
                        p.StreamId = _Battle.Battle_ID;
                        p.ReplayData = ReplayInfo;
                        p.BattleData = BattleData;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task Save(Level avatar)
        {
            try
            {
                string Object = JsonConvert.SerializeObject(avatar.Avatar, this._settings);
                if (Constants.UseCacheServer)
                    Redis.Players.StringSet(avatar.Avatar.UserID.ToString(), Object + "#:#:#:#" + avatar.SaveToJSON(), TimeSpan.FromHours(4));

                using (Mysql context = new Mysql())
                {
                    Player p = await context.Player.FindAsync(avatar.Avatar.UserID);
                    if (p != null)
                    {
                        p.Avatar = Object;
                        p.GameObjects = avatar.SaveToJSON();
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task Save(List<Level> avatars, Save Save = Logic.Enums.Save.Mysql)
        {
            try
            {
                switch (Save)
                {
                    case Save.Redis:
                        {
                            foreach (Level pl in avatars)
                            {
                                Redis.Players.StringSet(pl.Avatar.UserID.ToString(), JsonConvert.SerializeObject(pl.Avatar, this._settings) + "#:#:#:#" + pl.SaveToJSON(), TimeSpan.FromHours(4));
                            }
                            break;
                        }

                    case Save.Mysql:
                        {
                            using (Mysql context = new Mysql())
                            {
                                foreach (Level pl in avatars)
                                {
                                    Player p = context.Player.Find(pl.Avatar.UserID);
                                    if (p != null)
                                    {

                                        p.Avatar = JsonConvert.SerializeObject(pl.Avatar, this._settings);
                                        p.GameObjects = pl.SaveToJSON();
                                    }

                                }
                                await context.SaveChangesAsync();
                            }
                            break;
                        }
                    case Save.Both:
                        {
                            this.Save(avatars, Save.Mysql);
                            this.Save(avatars, Save.Redis);
                            break;
                        }
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task Save(List<Alliance> alliances, Save Save = Logic.Enums.Save.Mysql)
        {
            try
            {
                switch (Save)
                {

                    case Save.Redis:
                        {
                            foreach (Alliance alliance in alliances)
                            {
                                Redis.Clans.StringSet(alliance.AllianceID.ToString(), alliance.SaveToJSON(), TimeSpan.FromHours(4));
                            }
                            break;
                        }
                    case Save.Mysql:
                        {
                            using (Mysql context = new Mysql())

                            {
                                foreach (Alliance alliance in alliances)
                                {
                                    Clan c = context.Clan.Find((int)alliance.AllianceID);
                                    if (c != null)
                                    {
                                        c.LastUpdateTime = DateTime.Now;
                                        c.Data = alliance.SaveToJSON();
                                    }

                                }
                                await context.SaveChangesAsync();
                            }
                            break;
                        }
                    case Save.Both:
                        {
                            this.Save(alliances, Save.Mysql);
                            this.Save(alliances, Save.Redis);
                            break;
                        }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}