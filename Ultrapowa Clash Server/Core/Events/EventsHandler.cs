
namespace UCS.Core.Events
{

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using System.Linq;
    using System.Threading;

    internal class EventsHandler
    {
        internal static EventHandler EHandler;

        internal delegate void EventHandler(Logic.Enums.Exits Type = Logic.Enums.Exits.CTRL_CLOSE_EVENT);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsHandler"/> class.
        /// </summary>
        internal EventsHandler()
        {
            EventsHandler.EHandler += this.Handler;
            EventsHandler.SetConsoleCtrlHandler(EventsHandler.EHandler, true);
        }
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler Handler, bool Enabled);

        internal void ExitHandler()
        {
            try
            {
                if (ResourcesManager.GetInMemoryLevels().Count > 0)
                {
                    Parallel.ForEach(ResourcesManager.GetInMemoryLevels(), (_Player) =>
                    {
                        if (_Player != null)
                        {
                            ResourcesManager.LogPlayerOut(_Player);
                        }
                    });
                }


                if (ResourcesManager.GetInMemoryAlliances().Count > 0)
                {
                    Parallel.ForEach(ResourcesManager.GetInMemoryAlliances(), (_Player) =>
                    {
                        if (_Player != null)
                        {
                            ResourcesManager.RemoveAllianceFromMemory(_Player.GetAllianceId());
                        }
                    });
                }
                /*lock (Resources.Players.Gate)
                {
                    if (Resources.Players.Count > 0)
                    {
                        List<Player> Players = Resources.Players.Values.ToList();

                        Parallel.ForEach(Players, (_Player) =>
                        {
                            if (_Player != null)
                            {
                                Resources.Players.Save(_Player, Constants.Database);
                                Resources.Players.Remove(_Player);
                                Redis.Players.KeyDelete(_Player.LowID.ToString());
                            }
                        });
                    }
                }

                lock (Resources.Clans.Gate)
                {
                    if (Resources.Clans.Count > 0)
                    {
                        List<Clan> Clans = Resources.Clans.Values.ToList();

                        foreach (Clan _Clan in Clans)
                        {
                            if (_Clan != null)
                            {
                                Resources.Clans.Save(_Clan, Constants.Database);
                                Resources.Clans.Remove(_Clan);
                                Redis.Clans.KeyDelete(_Clan.LowID.ToString());
                            }
                        }
                    }
                }*/
            }
            catch (Exception)
            {
                Logger.Error("Failed to save all Players and Alliances!");
            }
        }

        internal void Handler(Logic.Enums.Exits Type = Logic.Enums.Exits.CTRL_CLOSE_EVENT)
        {
            Logger.Say("UCS is shutting down", true);
            new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                }
            }).Start();
            this.ExitHandler();
        }
    }
}