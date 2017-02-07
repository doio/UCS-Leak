using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core.Network;
using UCS.Core.Threading;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets;
using UCS.Packets.Messages.Server;

namespace UCS.Core
{
    internal class ResourcesManager : IDisposable
    {
        private static ConcurrentDictionary<long, Client> m_vClients             = null;
        private static ConcurrentDictionary<long, Level> m_vInMemoryLevels       = null;
        private static ConcurrentDictionary<long, Alliance> m_vInMemoryAlliances = null;
        private static List<Level> m_vOnlinePlayers                              = null;
        private static DatabaseManager m_vDatabase                               = null;

        public ResourcesManager()
        {
            m_vDatabase = new DatabaseManager();
            m_vOnlinePlayers = new List<Level>();
            m_vClients = new ConcurrentDictionary<long, Client>();
            m_vInMemoryLevels = new ConcurrentDictionary<long, Level>();
            m_vInMemoryAlliances = new ConcurrentDictionary<long, Alliance>();
        }

        public static void AddClient(Socket _Socket)
        {
            Client c = new Client(_Socket);
            c.CIPAddress = ((System.Net.IPEndPoint)_Socket.RemoteEndPoint).Address.ToString();
            m_vClients.TryAdd(c.Socket.Handle.ToInt64(), c);
        }

        public static void DropClient(long socketHandle)
        {
            try
            {
                Client _Client = null;
                m_vClients.TryRemove(socketHandle, out _Client);
                if (_Client.GetLevel() != null)
                {
                    LogPlayerOut(_Client.GetLevel());
                }
            }
            catch (Exception e)
            {
            }
        }

        public static List<long> GetAllPlayerIds()
        {
            return m_vDatabase.GetAllPlayerIds();
        }

        public static Client GetClient(long socketHandle)
        {
            return m_vClients.ContainsKey(socketHandle) ? m_vClients[socketHandle] : null;
        }

        public static List<Client> GetConnectedClients()
        {
            return m_vClients.Values.ToList();
        }

        public static List<Level> GetInMemoryLevels()
        {
            return m_vInMemoryLevels.Values.ToList();
        }

        public static List<Level> GetOnlinePlayers()
        {
            return m_vOnlinePlayers;
        }

        public static Level GetPlayer(long id, bool persistent = false)
        {
            Level result = GetInMemoryPlayer(id);
            if (result == null)
            {
                result = m_vDatabase.GetAccount(id);
                if (persistent)
                {
                    LoadLevel(result);
                }
            }
            return result;
        }

        public static void DisconnectClient(Client c)
        {
            PacketProcessor.Send(new OutOfSyncMessage(c));
            DropClient(c.GetSocketHandle());
        }

        public static bool IsClientConnected(long socketHandle)
        {
            return m_vClients[socketHandle] != null && m_vClients[socketHandle].IsClientSocketConnected();
        }

        public static Level GetPlayerWithFacebookID(string id)
        {
            foreach (Level p in GetInMemoryLevels())
            {
                if (p.GetPlayerAvatar().GetFacebookID() == id)
                {
                    return p;
                }
                else
                {
                    for (long i = 1; i < ObjectManager.GetMaxPlayerID(); i++)
                    {
                        Level l = m_vDatabase.GetAccount(i);

                        if (l.GetPlayerAvatar().GetFacebookID() == id)
                        {
                            return l;
                            break;
                        }
                    }
                }
            }
            return null;
        }

        public static bool IsPlayerOnline(Level l)
        {
            return m_vOnlinePlayers.Contains(l);
        }

        public static void LoadLevel(Level level)
        {
            m_vInMemoryLevels.TryAdd(level.GetPlayerAvatar().GetId(), level);
        }

        public static void LogPlayerIn(Level l, Client c)
        {
            l.SetClient(c);
            c.SetLevel(l);

            if (!m_vOnlinePlayers.Contains(l))
            {
                m_vOnlinePlayers.Add(l);
                LoadLevel(l);
                Program.TitleU();
            }
            else
            {
                int i = m_vOnlinePlayers.IndexOf(l);
                m_vOnlinePlayers[i] = l;
            }
        }

        public static void LogPlayerOut(Level level)
        {
            DatabaseManager.Single().Save(level);
            m_vOnlinePlayers.Remove(level);
            m_vInMemoryLevels.TryRemove(level.GetPlayerAvatar().GetId());
            m_vClients.TryRemove(level.GetClient().GetSocketHandle());
            Program.TitleD();
        }

        private static Level GetInMemoryPlayer(long id)
        {
            return m_vInMemoryLevels.ContainsKey(id) ? m_vInMemoryLevels[id] : null;
        }

        public static List<Alliance> GetInMemoryAlliances()
        {
            return m_vInMemoryAlliances.Values.ToList();
        }

        public static void AddAllianceInMemory(Alliance all)
        {
            m_vInMemoryAlliances.TryAdd(all.GetAllianceId(), all);
        }

        public static void AddAllianceInMemory(List<Alliance> all)
        {
            for (int i = 0, allCount = all.Count; i < allCount; i++)
            {
                Alliance a = all[i];
                m_vInMemoryAlliances.TryAdd(a.GetAllianceId(), a);
            }
        }

        public static bool InMemoryAlliancesContain(long key)
        {
            return m_vInMemoryAlliances.Keys.Contains(key);
        }

        public static bool InMemoryAlliancesContain(Alliance all)
        {
            return m_vInMemoryAlliances.Values.Contains(all);
        }

        public static Alliance GetInMemoryAlliance(long key)
        {
            Alliance a;
            m_vInMemoryAlliances.TryGetValue(key, out a);
            return a;
        }

        public static void RemoveAllianceFromMemory(long key)
        {
            m_vInMemoryAlliances.TryRemove(key);
        }

        public static void SetGameObject(Level level, string json)
        {
            level.GetHomeOwnerAvatar().LoadFromJSON(json);

            LogPlayerOut(level);
        }

        public void Dispose()
        {
            GetInMemoryAlliances().Clear();
            GetInMemoryLevels().Clear();
            GetOnlinePlayers().Clear();
        }
    }
}