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
            }
        }

        public Level GetAccount(long playerId)
        {
            Level account = null;
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    player p = db.player.Find(playerId);

                    if (p != null)
                    {
                        account = new Level();
                        account.SetAccountStatus(p.AccountStatus);
                        account.SetAccountPrivileges(p.AccountPrivileges);
                        account.SetTime(p.LastUpdateTime);
                        account.SetIPAddress(p.IPAddress);
                        account.GetPlayerAvatar().LoadFromJSON(p.Avatar);
                        account.LoadFromJSON(p.GameObjects);
                    }
                }
            }
            catch (Exception ex)
            {
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
                    int count = 0;
                    foreach (clan c in a)
                    {
                        if (count++ >= 100)
                            break;
                        Alliance alliance = new Alliance(c.ClanId);
                        alliance.LoadFromJSON(c.Data);
                        alliances.Add(alliance);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return alliances;
        }

        public Alliance GetAlliance(long allianceId)
        {
            Alliance alliance = null;
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    clan p = db.clan.Find(allianceId);
                    if (p != null)
                    {
                        alliance = new Alliance();
                        alliance.LoadFromJSON(p.Data);
                    }
                }
            }
            catch (Exception)
            {
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

        public long GetMaxAllianceId()
        {
            long max = 0;
            using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                max = (from alliance in db.clan select (long?)alliance.ClanId ?? 0).DefaultIfEmpty().Max();
            return max;
        }

        public long GetMaxPlayerId()
        {
            try
            {
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                    return (from ep in db.player select (long?)ep.PlayerId ?? 0).DefaultIfEmpty().Max();
            }
            catch (MySqlException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[UCS]    Error when loging in to the MySQL Server.  Possible Issues :");
                Console.WriteLine("[UCS]        -> Username is wrong");
                Console.WriteLine("[UCS]        -> Password is wrong");
                Console.WriteLine("[UCS]        -> Your IP Address is unauthorized");
                Console.WriteLine("[UCS]    UCS Emulator is now closing...");
                Console.ResetColor();
                Thread.Sleep(8000);
                Environment.Exit(0);
            }
            catch (InvalidOperationException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[UCS]    Error when reading the appconfig file.  Possible Issues :");
                Console.WriteLine("[UCS]        -> File is missing");
                Console.WriteLine("[UCS]        -> You edited the file wrongly");
                Console.WriteLine("[UCS]        -> You moved the file");
                Console.WriteLine("[UCS]    UCS Emulator is now closing...");
                Console.ResetColor();
                Thread.Sleep(8000);
                Environment.Exit(0);
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[UCS]    Error when connecting to your MySQL Server.  Possible Issues :");
                Console.WriteLine("[UCS]        -> Wrong IP");
                Console.WriteLine("[UCS]        -> Wrong User");
                Console.WriteLine("[UCS]        -> Wrong Password");
                Console.WriteLine("[UCS]        -> Wrong DB Name");
                Console.WriteLine("[UCS]        -> Or you simply didnt imported the tables");
                Console.WriteLine("[UCS]    UCS Emulator is now closing...");
                Console.ResetColor();
                Thread.Sleep(8000);
                Environment.Exit(0);
            }
            return 0;
        }

        public void RemoveAlliance(Alliance alliance)
        {
            try
            {
                long id = alliance.GetAllianceId();
                using (ucsdbEntities db = new ucsdbEntities(m_vConnectionString))
                {
                    db.clan.Remove(db.clan.Find(id));
                    db.SaveChanges();
                }
                ObjectManager.RemoveInMemoryAlliance(id);
            }
            catch (Exception)
            {
            }
        }

        public void Save(Alliance alliance)
        {
            try
            {
                using (ucsdbEntities context = new ucsdbEntities(m_vConnectionString))
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    clan c = context.clan.Find(alliance.GetAllianceId());
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
                            }
                            );
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetGameObject(Level avatar, string json)
        {
            try
            {
                using (ucsdbEntities context = new ucsdbEntities(m_vConnectionString))
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    player p = context.player.Find(avatar.GetPlayerAvatar().GetId());
                    if (p != null)
                    {
                        p.LastUpdateTime = avatar.GetTime();
                        p.AccountStatus = avatar.GetAccountStatus();
                        p.AccountPrivileges = avatar.GetAccountPrivileges();
                        p.IPAddress = avatar.GetIPAddress();
                        p.Avatar = avatar.GetPlayerAvatar().SaveToJSON();
                        p.GameObjects = json;
                        context.Entry(p).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public void Save(Level avatar)
        {
            try
            {
                using (ucsdbEntities context = new ucsdbEntities(m_vConnectionString))
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    player p = context.player.Find(avatar.GetPlayerAvatar().GetId());
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
                            }
                            );
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public void Save(List<Level> avatars)
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
                            {
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
                                   }
                                );
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Save(List<Alliance> alliances)
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
                                        Data = alliance.SaveToJSON()
                                    }
                                );
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}