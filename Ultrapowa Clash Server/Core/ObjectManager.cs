using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UCS.Core.Network;
using UCS.Files;
using UCS.Files.CSV;
using UCS.Files.Logic;
using UCS.Logic;
using UCS.Packets.Messages.Server;
using Timer = System.Threading.Timer;
using static UCS.Core.Logger;
using UCS.Core.Threading;

namespace UCS.Core
{
    internal class ObjectManager : IDisposable
    {
        private static long m_vAllianceSeed;
        private static long m_vAvatarSeed;
        public static int m_vDonationSeed;
        private static int m_vRandomBaseAmount;
        private static DatabaseManager m_vDatabase;
        private static string m_vHomeDefault;
        public static bool m_vTimerCanceled;
        public static Timer TimerReference;
        public static Dictionary<int, string> NpcLevels;
        public static Dictionary<int, string> m_vRandomBases;
        public static FingerPrint FingerPrint;
        static int MaxPlayerID;
        static int MaxAllianceID;

        public ObjectManager()
        {
            m_vTimerCanceled = false;

            m_vDatabase = new DatabaseManager();
            NpcLevels = new Dictionary<int, string>();
            m_vRandomBases = new Dictionary<int, string>();
            FingerPrint = new FingerPrint();

            MaxPlayerID = Convert.ToInt32(m_vDatabase.GetMaxPlayerId() + 1);
            MaxAllianceID = Convert.ToInt32(m_vDatabase.GetMaxAllianceId() + 1);

            m_vAvatarSeed = MaxPlayerID;
            m_vAllianceSeed = MaxAllianceID;
            using (StreamReader sr = new StreamReader(@"Gamefiles/starting_home.json"))
                m_vHomeDefault = sr.ReadToEnd();

            LoadNpcLevels();
            //LoadRandomBase(); // Useless atm

            TimerReference = new Timer(Save, null, 0, 30000);
            Say("UCS Database has been succesfully loaded. (" + Convert.ToInt32(MaxAllianceID + MaxPlayerID) + "_Tables)");
        }

        private void Save(object state)
        {
            m_vDatabase.Save(ResourcesManager.GetInMemoryLevels());
            m_vDatabase.Save(ResourcesManager.GetInMemoryAlliances());

            if (m_vTimerCanceled)
            {
                TimerReference.Dispose();
            }
        }

        public static Alliance CreateAlliance(long seed)
        {
            Alliance alliance;
            if (seed == 0)
                seed = m_vAllianceSeed;
            alliance = new Alliance(seed);
            m_vAllianceSeed++;
            m_vDatabase.CreateAlliance(alliance);
            ResourcesManager.AddAllianceInMemory(alliance);
            return alliance;
        }

        public static Level CreateAvatar(long seed, string token)
        {
            Level pl;
            if (seed == 0)
            {
                seed = m_vAvatarSeed;
            }
            pl = new Level(seed, token);
            m_vAvatarSeed++;
            pl.LoadFromJSON(m_vHomeDefault);
            m_vDatabase.CreateAccount(pl);
            return pl;
        }

        public static void LoadAllAlliancesFromDB()
        {
            ResourcesManager.AddAllianceInMemory(m_vDatabase.GetAllAlliances());
        }

        public static Alliance GetAlliance(long allianceId)
        {
            Alliance alliance;
            if (ResourcesManager.InMemoryAlliancesContain(allianceId))
                return ResourcesManager.GetInMemoryAlliance(allianceId);
            else
            {
                alliance = m_vDatabase.GetAlliance(allianceId);
                if (alliance != null)
                    ResourcesManager.AddAllianceInMemory(alliance);
                else
                    return null;
                return alliance;
            }
        }

        public static List<Alliance> GetInMemoryAlliances() => ResourcesManager.GetInMemoryAlliances();

        public static Level GetRandomOnlinePlayer()
        {
            int index = new Random().Next(0, ResourcesManager.GetInMemoryLevels().Count);
            return ResourcesManager.GetInMemoryLevels().ElementAt(index);
        }

        public static Level GetRandomOnlinePlayerWithoutShield()
        {
            Level l = GetRandomOnlinePlayer();
            ClientAvatar ca = l.GetPlayerAvatar();

            if (l != null) // Have to find a better method to find Players without Shield
            {
                return l;
            }
            else
            {
                return GetRandomOnlinePlayerWithoutShield(); // Insane loop?
            }
        }

        public static Level GetRandomPlayerFromAll()
        {
            int index = new Random().Next(0, ResourcesManager.GetAllPlayerIds().Count);
            return ResourcesManager.GetPlayer(ResourcesManager.GetAllPlayerIds()[index]);
        }

        public static void LoadNpcLevels()
        {
            int Size = 0;
            NpcLevels.Add(17000000, new StreamReader(@"Gamefiles/level/NPC/tutorial_npc.json").ReadToEnd());
            NpcLevels.Add(17000001, new StreamReader(@"Gamefiles/level/NPC/tutorial_npc2.json").ReadToEnd());
            for (int i = 2; i < 50; i++)
                using (StreamReader sr = new StreamReader(@"Gamefiles/level/NPC/level" + (i + 1) + ".json"))
                    NpcLevels.Add(i + 17000000, sr.ReadToEnd());
            foreach(var s in NpcLevels)
            {
                Size += System.Text.Encoding.UTF8.GetByteCount(s.Value);
            }
            Say("NPC Levels  have been succesfully loaded. (" + Size + ")");
        }

        public static void LoadRandomBase()
        {
            m_vRandomBaseAmount = Directory.GetFiles(@"Gamefiles/level/PVP", "Base*.json").Count();
            for (int i = 0; i < m_vRandomBaseAmount; i++)
                using (StreamReader sr2 = new StreamReader(@"Gamefiles/level/PVP/Base" + (i + 1) + ".json"))
                    m_vRandomBases.Add(i, sr2.ReadToEnd());
            Say("PVP Levels  have been succesfully loaded.");
        }

        public static void RemoveInMemoryAlliance(long id)
        {
            ResourcesManager.RemoveAllianceFromMemory(id);
        }

        public static int GetMaxAllianceID() => MaxAllianceID;

        public static int GetMaxPlayerID() => MaxPlayerID;

        public static int RandomBaseCount() => m_vRandomBaseAmount;

        public void Dispose()
        {
            if (TimerReference != null)
            {
                TimerReference.Dispose();
                TimerReference = null;
            }
        }

    }
}
