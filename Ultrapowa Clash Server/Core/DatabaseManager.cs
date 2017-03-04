using System;
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

namespace UCS.Core
{
    internal class DatabaseManager
    {
        private readonly string m_vConnectionString;

        public DatabaseManager()
        {
            m_vConnectionString = ConfigurationManager.AppSettings["databaseConnectionName"];
        }

        public static DatabaseManager Single() => new DatabaseManager();

        public void CreateAccount(Level l)
        {
            try
            {
                if (Constants.UseCacheServer) //Redis As Cache Server
                    Redis.Players.StringSet(l.Avatar.GetId().ToString(), l.Avatar.SaveToJSON() + "#:#:#:#" + l.SaveToJSON(),
                        TimeSpan.FromHours(4));

                using (Mysql db = new Mysql(m_vConnectionString))
                {
                    db.Player.Add(
                        new Player
                        {
                            PlayerId = l.Avatar.GetId(),
                            Avatar = l.Avatar.SaveToJSON(),
                            GameObjects = l.SaveToJSON()
                        }
                    );
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public void CreateAlliance(Alliance a)
        {
            try
            {
                if (Constants.UseCacheServer) //Redis As Cache Server
                    Redis.Clans.StringSet(a.GetAllianceId().ToString(), a.SaveToJSON(), TimeSpan.FromHours(4));

                using (Mysql db = new Mysql(m_vConnectionString))
                {
                    db.Clan.Add(
                        new Clan()
                        {
                            ClanId = a.GetAllianceId(),
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
                            account = new Level();
                            account.Avatar.LoadFromJSON(_Datas[0]);
                            account.LoadFromJSON(_Datas[1]);
                        }
                    }
                    else
                    {
                        using (Mysql db = new Mysql(m_vConnectionString))
                        {
                            Player p = await db.Player.FindAsync(playerId);

                            if (p != null)
                            {
                                account = new Level();
                                account.Avatar.LoadFromJSON(p.Avatar);
                                account.LoadFromJSON(p.GameObjects);
                                Redis.Players.StringSet(playerId.ToString(), p.Avatar + "#:#:#:#" + p.GameObjects,
                                    TimeSpan.FromHours(4));
                            }
                        }
                        ;
                    }
                }
                else
                {
                    using (Mysql db = new Mysql(m_vConnectionString))
                    {
                        Player p = await db.Player.FindAsync(playerId);

                        if (p != null)
                        {
                            account = new Level();
                            account.Avatar.LoadFromJSON(p.Avatar);
                            account.LoadFromJSON(p.GameObjects);
                        }
                    }
                }
                return account;
            }
            catch (Exception)
            {
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
                        using (Mysql db = new Mysql(m_vConnectionString))
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
                    using (Mysql db = new Mysql(m_vConnectionString))
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
            using (Mysql db = new Mysql(m_vConnectionString))
                ids.AddRange(db.Player.Select(p => p.PlayerId));
            return ids;
        }

        public List<long> GetAllClanIds()
        {
            List<long> ids = new List<long>();
            using (Mysql db = new Mysql(m_vConnectionString))
                ids.AddRange(db.Clan.Select(p => p.ClanId));
            return ids;
        }

        public long GetMaxAllianceId()
        {
            try
            {
                using (Mysql db = new Mysql(m_vConnectionString))
                    return (from alliance in db.Clan select (long?) alliance.ClanId ?? 0).DefaultIfEmpty().Max();
            }
            catch
            {
                return 0;
            }
        }

        public long GetMaxPlayerId()
        {
            try
            {
                using (Mysql db = new Mysql(m_vConnectionString))
                    return (from ep in db.Player select (long?) ep.PlayerId ?? 0).DefaultIfEmpty().Max();
            }
            catch (EntityException ex)
            {
                if (ConfigurationManager.AppSettings["databaseConnectionName"] == "mysql")
                {
                    Error("An exception occured when connecting to the MySQL Server.");
                    Error("Please check your database configuration !");
                    Error(Convert.ToString(ex));
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                {
                    Error("An exception occured when connecting to the SQLite database.");
                    Error("Please check your database configuration !");
                    Error(Convert.ToString(ex));
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            catch (MySqlException)
            {
                Say();
                Error("An exception occured when reconnecting to the MySQL Server.");
                Error("Please check your database configuration !");
                //Reason
                //Username is wrong
                //Password is wrong
                //IP Address is unauthorized

                UCSControl.UCSRestart();
            }
            catch (Exception ex)
            {
                Error("An unknown exception occured when trying to connect to the sql server.");
                Error("Please check your database configuration !");
                Error(Convert.ToString(ex));
                Console.ReadKey();
                Environment.Exit(0);
            }
            return 0;
        }

        public void RemoveAlliance(Alliance alliance)
        {
            try
            {
                long id = alliance.GetAllianceId();
                using (Mysql db = new Mysql(m_vConnectionString))
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
                using (Mysql Database = new Mysql(m_vConnectionString))
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
                        account = new Level();
                        account.Avatar.LoadFromJSON(Data.Avatar);
                        account.LoadFromJSON(Data.GameObjects);
                        if (Constants.UseCacheServer)
                            Redis.Players.StringSet(Data.PlayerId.ToString(), Data.Avatar + "#:#:#:#" + Data.GameObjects,
                                TimeSpan.FromHours(4));
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
                    Redis.Clans.StringSet(alliance.GetAllianceId().ToString(), alliance.SaveToJSON(), TimeSpan.FromHours(4));

                using (Mysql context = new Mysql(m_vConnectionString))
                {
                    Clan c = await context.Clan.FindAsync((int)alliance.GetAllianceId());
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

        public async Task Save(Level avatar)
        {
            try
            {
                if (Constants.UseCacheServer)
                    Redis.Players.StringSet(avatar.Avatar.GetId().ToString(),
                        avatar.Avatar.SaveToJSON() + "#:#:#:#" + avatar.SaveToJSON(), TimeSpan.FromHours(4));

                using (Mysql context = new Mysql(m_vConnectionString))
                {
                    Player p = await context.Player.FindAsync(avatar.Avatar.GetId());
                    if (p != null)
                    {
                        p.Avatar = avatar.Avatar.SaveToJSON();
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
                                Redis.Players.StringSet(pl.Avatar.GetId().ToString(),
                                    pl.Avatar.SaveToJSON() + "#:#:#:#" + pl.SaveToJSON(), TimeSpan.FromHours(4));
                            }
                            break;
                        }

                    case Save.Mysql:
                        {
                            using (Mysql context = new Mysql(m_vConnectionString))
                            {
                                foreach (Level pl in avatars)
                                {
                                    Player p = context.Player.Find(pl.Avatar.GetId());
                                    if (p != null)
                                    {

                                        p.Avatar = pl.Avatar.SaveToJSON();
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
                                Redis.Clans.StringSet(alliance.GetAllianceId().ToString(), alliance.SaveToJSON(),
                                    TimeSpan.FromHours(4));
                            }
                            break;
                        }
                    case Save.Mysql:
                        {
                            using (Mysql context = new Mysql(m_vConnectionString))
                            {
                                foreach (Alliance alliance in alliances)
                                {
                                    Clan c = context.Clan.Find((int)alliance.GetAllianceId());
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