using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UCS.Core;
using UCS.Core.Network;
using UCS.Core.Settings;
using UCS.Core.Threading;
using UCS.Helpers;
using UCS.WebAPI;

namespace UCS
{
    class Program
    {
        public static int OP = 0;

        public const string Title = "Ultrapowa Clash Server v0.7.3.0 - © 2017 | Online Players: ";

        public static Stopwatch _Stopwatch = new Stopwatch();

        internal static void Main(string[] args)
        {
            new Thread(() =>
            {
                _Stopwatch.Start();

                Console.Title = Title + OP;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(

                    @"
      ____ ___.__   __                                              
     |    |   \  |_/  |_____________  ______   ______  _  _______   
     |    |   /  |\   __\_  __ \__  \ \____ \ /  _ \ \/ \/ /\__  \  
     |    |  /|  |_|  |  |  | \// __ \|  |_> >  <_> )     /  / __ \_
     |______/ |____/__|  |__|  (____  /   __/ \____/ \/\_/  (____  /
                                    \/|__|                       \/
            ");
                Console.ResetColor();
                Console.WriteLine("[UCS]    > This program is made by the Ultrapowa Development Team.\n[UCS]    > Ultrapowa is not affiliated to \"Supercell, Oy\".\n[UCS]    > This program is copyrighted worldwide.\n[UCS]    > Visit www.ultrapowa.com daily for News & Updates!");
                if (Constants.IsRc4)
                    Console.WriteLine("[UCS]    > UCS is running under RC4 mode. Please make sure CSV is modded to allow RC4.");
                else
                    Console.WriteLine("[UCS]    > UCS is running under Pepper mode. Please make sure client key is modded.");
                Console.Write("[UCS]    ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("> UCS is up-to-date: " + Constants.Version);
                Console.ResetColor();
                Console.WriteLine("\n[UCS]    Prepearing Server...\n");

                if (Utils.ParseConfigBoolean("UseWebAPI"))
                {
                    new API();
                }

                new CheckThread();
                new MemoryThread();
                new NetworkThread();
                new ParserThread();
                //new GlobalChatThread();
            }).Start();
        }

        public static void TitleU()
        {
            Console.Title = Title + ResourcesManager.GetOnlinePlayers().Count.ToString();
        }

        public static void TitleD()
        {
            Console.Title = Title + ResourcesManager.GetOnlinePlayers().Count.ToString();
        }
    }
}
