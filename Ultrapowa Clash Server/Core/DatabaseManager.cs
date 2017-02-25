using System;
using System.Collections.Concurrent;
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
using System.Threading;

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
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    db.player.Add(
                        new player
                        {
                            PlayerId = l.GetPlayerAvatar().GetId(),
                            AccountStatus = l.GetAccountStatus(),
                            AccountPrivileges = l.GetAccountPrivileges(),
                            LastUpdateTime = l.GetTime(),
                            IPAddress = l.GetIPAddress(),
                            Avatar = l.GetPlayerAvatar().SaveToJSON(),
                            GameObjects = l.SaveToJSON()
                        }
                        );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to Create an Account " + ex);
            }
        }

        public void CreateAlliance(Alliance a)
        {
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    db.clan.Add(
                        new clan
                        {
                            ClanId = a.GetAllianceId(),
                            LastUpdateTime = DateTime.Now,
                            Data = a.SaveToJSON()
                        }
                        );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to Create an Alliance " + ex);
            }
        }

        public async Task<Level> GetAccount(long playerId)
        {
            Level account = null;
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    player p = await db.player.FindAsync(playerId);

                    if (p != null)
                    {
                        account = new Level();
                        account.SetAccountStatus(p.AccountStatus);
                        account.SetAccountPrivileges(p.AccountPrivileges);
                        account.SetTime(p.LastUpdateTime);
                        account.GetPlayerAvatar().LoadFromJSON(p.Avatar);
                        account.LoadFromJSON(p.GameObjects);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to get an Account " + ex);
            }
            return account;
        }

        public List<Alliance> GetAllAlliances()
        {
            List<Alliance> alliances = new List<Alliance>();
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    DbSet<clan> a = db.clan;
                    foreach (clan c in a)
                    {
                        Alliance alliance = new Alliance();
                        alliance.LoadFromJSON(c.Data);
                        alliances.Add(alliance);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to get 100 Alliances: " + ex);
            }
            return alliances;
        }

        public List<Alliance> Get100Alliances()
        {
            List<Alliance> alliances = new List<Alliance>();
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    DbSet<clan> a = db.clan;
                    int count = 0;
                    foreach (clan c in a)
                    {
                        Alliance alliance = new Alliance();
                        alliance.LoadFromJSON(c.Data);
                        alliances.Add(alliance);
                        if (count++ >= 100)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to get 100 Alliances: " + ex);
            }
            return alliances;
        }

        public ConcurrentDictionary<long, Level> GetAllPlayers()
        {
            ConcurrentDictionary<long, Level> players = new ConcurrentDictionary<long, Level>();
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    DbSet<player> a = db.player;
                    int count = 0;
                    foreach (player c in a)
                    {
                        Level pl = new Level();
                        players.TryAdd(pl.GetPlayerAvatar().GetId(), pl);
                        if (count++ >= 100)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to get All Players: " + ex);
            }
            return players;
        }

        public async Task<Alliance> GetAlliance(long allianceId)
        {
            Alliance alliance = null;
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    clan p = await db.clan.FindAsync(allianceId);
                    if (p != null)
                    {
                        alliance = new Alliance();
                        alliance.LoadFromJSON(p.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Error when try to get an Alliance: " + ex);
            }
            return alliance;
        }

        public List<long> GetAllPlayerIds()
        {
            List<long> ids = new List<long>();
            using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                ids.AddRange(db.player.Select(p => p.PlayerId));
            return ids;
        }
        public List<long> GetAllClanIds()
        {
            List<long> ids = new List<long>();
            using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                ids.AddRange(db.clan.Select(p => p.ClanId));
            return ids;
        }

        public long GetMaxAllianceId()
        {
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                    return (from alliance in db.clan select (long?)alliance.ClanId ?? 0).DefaultIfEmpty().Max();
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
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                    return (from ep in db.player select (long?)ep.PlayerId ?? 0).DefaultIfEmpty().Max();
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
            long id = alliance.GetAllianceId();
            using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
            {
                db.clan.Remove(db.clan.Find((int)id));
                db.SaveChanges();
            }
            ObjectManager.RemoveInMemoryAlliance(id);
        }

        public async Task Save(Alliance alliance)
        {
            using (ucsdbEntities context = new ucsdbEntities(m_vConnectionString))
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                clan c = await context.clan.FindAsync((int)alliance.GetAllianceId());
                if (c != null)
                {
                    c.LastUpdateTime = DateTime.Now;
                    c.Data = alliance.SaveToJSON();
                    context.Entry(c).State = EntityState.Modified;
                }
                else
                {
                    context.clan.Add(
                        new clan
                        {
                            ClanId = alliance.GetAllianceId(),
                            LastUpdateTime = DateTime.Now,
                            Data = alliance.SaveToJSON()
                        });
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task Save(Level avatar)
        {
            ucsdbEntities context = new ucsdbEntities(m_vConnectionString);
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                player p = await context.player.FindAsync(avatar.GetPlayerAvatar().GetId());
                if (p != null)
                {
                    p.LastUpdateTime = avatar.GetTime();
                    p.AccountStatus = avatar.GetAccountStatus();
                    p.AccountPrivileges = avatar.GetAccountPrivileges();
                    p.IPAddress = avatar.GetIPAddress();
                    p.Avatar = avatar.GetPlayerAvatar().SaveToJSON();
                    p.GameObjects = avatar.SaveToJSON();
                    context.Entry(p).State = EntityState.Modified;
                }
                else
                {
                    context.player.Add(
                        new player
                        {
                            PlayerId = avatar.GetPlayerAvatar().GetId(),
                            AccountStatus = avatar.GetAccountStatus(),
                            AccountPrivileges = avatar.GetAccountPrivileges(),
                            LastUpdateTime = avatar.GetTime(),
                            IPAddress = avatar.GetIPAddress(),
                            Avatar = avatar.GetPlayerAvatar().SaveToJSON(),
                            GameObjects = avatar.SaveToJSON()
                        });
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task Save(List<Level> avatars)
        {
            try
            {
                using (ucsdbEntities context = new ucsdbEntities(m_vConnectionString))
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    foreach (Level pl in avatars)
                    {
                        lock (pl)
                        {
                            player p = context.player.Find(pl.GetPlayerAvatar().GetId());
                            if (p != null)
                            {
                                p.LastUpdateTime = pl.GetTime();
                                p.AccountStatus = pl.GetAccountStatus();
                                p.AccountPrivileges = pl.GetAccountPrivileges();
                                p.IPAddress = pl.GetIPAddress();
                                p.Avatar = pl.GetPlayerAvatar().SaveToJSON();
                                p.GameObjects = pl.SaveToJSON();
                                context.Entry(p).State = EntityState.Modified;
                            }
                            else
                                context.player.Add(
                                    new player
                                    {
                                        PlayerId = pl.GetPlayerAvatar().GetId(),
                                        AccountStatus = pl.GetAccountStatus(),
                                        AccountPrivileges = pl.GetAccountPrivileges(),
                                        LastUpdateTime = pl.GetTime(),
                                        IPAddress = pl.GetIPAddress(),
                                        Avatar = pl.GetPlayerAvatar().SaveToJSON(),
                                        GameObjects = pl.SaveToJSON()
                                    });
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch
            {
            }
        }

        public async Task Save(List<Alliance> alliances)
        {
            try
            {
                using (ucsdbEntities context = new ucsdbEntities(m_vConnectionString))
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    foreach (Alliance alliance in alliances)
                    {
                        lock (alliance)
                        {
                            clan c = context.clan.Find((int)alliance.GetAllianceId());
                            if (c != null)
                            {
                                c.LastUpdateTime = DateTime.Now;
                                c.Data = alliance.SaveToJSON();
                                context.Entry(c).State = EntityState.Modified;
                            }
                            else
                            {
                                context.clan.Add(
                                    new clan
                                    {
                                        ClanId = alliance.GetAllianceId(),
                                        LastUpdateTime = DateTime.Now,
                                        Data = alliance.SaveToJSON(),

                                    });
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch
            {
            }
        }
    }
}