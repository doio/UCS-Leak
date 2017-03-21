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
        internal static ConcurrentDictionary<IntPtr, Device> Clients = null;
        internal static ConcurrentDictionary<long, Level> InMemoryLevels = null;
        internal static ConcurrentDictionary<long, Battle> Battles = null;
        internal static ConcurrentDictionary<long, Alliance> InMemoryAlliances = null;
        internal static List<Level> OnlinePlayers = null;
        internal static DatabaseManager m_vDatabase = null;

        public ResourcesManager()
        {
            m_vDatabase = new DatabaseManager();
            OnlinePlayers = new List<Level>();
            Clients = new ConcurrentDictionary<IntPtr, Device>();
            InMemoryLevels = new ConcurrentDictionary<long, Level>();
            InMemoryAlliances = new ConcurrentDictionary<long, Alliance>();
            Battles = new ConcurrentDictionary<long, Battle>();
        }

        public static void AddClient(Socket _Socket)
        {
            Device c = new Device(_Socket){IPAddress = ((System.Net.IPEndPoint) _Socket.RemoteEndPoint).Address.ToString()};
            Clients.TryAdd(c.Socket.Handle, c);
        }
        public static void AddClient(Device Client)
        {
            Clients.TryAdd(Client.Socket.Handle, Client);
        }
        public static void DropClient(IntPtr socketHandle)
        {
            try
            {
                Device _Client = null;
                Clients.TryRemove(socketHandle, out _Client);
                if (_Client.Player != null)
                {
                    LogPlayerOut(_Client.Player);
                }
            }
            catch (Exception e)
            {
            }
        }
        public static void DropClient(Device client)
        {
            try
            {
                Clients.TryRemove(client.SocketHandle);
                if (client.Player != null)
                {
                    LogPlayerOut(client.Player);
                }
            }
            catch (Exception e)
            {
            }
        }

        public static List<long> GetAllPlayerIds() => m_vDatabase.GetAllPlayerIds();

        public static Device GetClient(IntPtr socketHandle) => Clients.ContainsKey(socketHandle) ? Clients[socketHandle] : null;

        public static List<Device> GetConnectedClients() => Clients.Values.ToList();

        public static List<Level> GetInMemoryLevels() => InMemoryLevels.Values.ToList();

        public static List<Level> GetOnlinePlayers() => OnlinePlayers;

        public static async Task<Level> GetPlayer(long id, bool persistent = false)
        {
            Level result = GetInMemoryPlayer(id);
            if (result == null)
            {
                result = await m_vDatabase.GetAccount(id);
                if (persistent)
                {
                    LoadLevel(result);
                }
            }
            return result;
        }

        public static void DisconnectClient(Device _Client)
        {
            Processor.Send(new OutOfSyncMessage(_Client));
            DropClient(_Client.SocketHandle);
        }

        public static bool IsClientConnected(IntPtr socketHandle) => Clients[socketHandle] != null && Clients[socketHandle].IsClientSocketConnected();

        public static bool IsPlayerOnline(Level l) => OnlinePlayers.Contains(l);

        public static void LoadLevel(Level level)
        {
            InMemoryLevels.TryAdd(level.Avatar.UserID, level);
        }

        public static void LogPlayerIn(Level l, Device c)
        {
            l.Client = c;
            c.Player = l;

            if (!OnlinePlayers.Contains(l))
            {
                OnlinePlayers.Add(l);
                LoadLevel(l);
                Program.TitleU();
            }
            else
            {
                int i = OnlinePlayers.IndexOf(l);
                OnlinePlayers[i] = l;
            }
        }

        public static void RemoveBattle(long id)
        {
            try
            {
                Battles.TryRemove(id);
            }
            catch (Exception e)
            {
            }
        }

        public static void LogPlayerOut(Level level)
        {
            DatabaseManager.Single().Save(level);
            OnlinePlayers.Remove(level);
            InMemoryLevels.TryRemove(level.Avatar.UserID);
            Clients.TryRemove(level.Client.SocketHandle);
            Program.TitleD();
        }

        internal static Battle GetInMemoryBattle(long id) => Battles.ContainsKey(id) ? Battles[id] : null;
        private static Level GetInMemoryPlayer(long id) => InMemoryLevels.ContainsKey(id) ? InMemoryLevels[id] : null;

        public static List<Alliance> GetInMemoryAlliances() => InMemoryAlliances.Values.ToList();

        public static void AddAllianceInMemory(Alliance all)
        {
            InMemoryAlliances.TryAdd(all.AllianceID, all);
        }
        public static void AddBattle(Battle _battle)
        {
            Battles.TryAdd(_battle.Battle_ID, _battle);
        }


        public static void AddAllianceInMemory(List<Alliance> all)
        {
            for (int i = 0, allCount = all.Count; i < allCount; i++)
            {
                Alliance a = all[i];
                InMemoryAlliances.TryAdd(a.AllianceID, a);
            }
        }

        public static bool InMemoryAlliancesContain(long key) => InMemoryAlliances.Keys.Contains(key);

        public static bool InMemoryAlliancesContain(Alliance all) => InMemoryAlliances.Values.Contains(all);

        public static Alliance GetInMemoryAlliance(long key)
        {
            Alliance a;
            InMemoryAlliances.TryGetValue(key, out a);
            return a;
        }

        public static void RemoveAllianceFromMemory(long key)
        {
            InMemoryAlliances.TryRemove(key);
        }

        public static void SetGameObject(Level level, string json)
        {
            level.LoadFromJSON(json);
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