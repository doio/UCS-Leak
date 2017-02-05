using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UCS.Core;
using UCS.Core.Network;
using UCS.Core.Settings;
using UCS.Core.Threading;
using UCS.Core.Web;
using UCS.Helpers;
using UCS.WebAPI;

namespace UCS
{
    class Program
    {
        public static int OP = 0;
        public const string Title = "Ultrapowa Clash Server v0.7.3.0 - © 2017 | Online Players: ";
        public static Stopwatch _Stopwatch = new Stopwatch();
        public static string Version { get; set; }

        internal static void Main(string[] args)
        {
            new Thread((ThreadStart)(() =>
            {
                _Stopwatch.Start();

                if (Constants.IsPremiumServer)
                {
                    Console.Title = Title + OP;
                }
                else
                {
                    Console.Title = Title + OP + "/200";
                }

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
                Version = VersionChecker.GetVersionString();

                if (Version == Constants.Version)
                {
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
                    new ChatProcessor();
                }
                else if (Version == "Error")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("> An Error occured when requesting the Version number.");
                    Console.WriteLine();
                    Logger.Say("Please contact the Support at https://ultrapowa.com/forum!");
                    Console.WriteLine();
                    Logger.Say("Aborting...");
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("> UCS is not up-to-date! New Version: " + VersionChecker.GetVersionString() + ". Aborting...");
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }
            })).Start();
        }

        public static void TitleU()
        {
            if (Constants.IsPremiumServer)
            {
                Console.Title = Title + ResourcesManager.GetOnlinePlayers().Count.ToString();
            }
            else
            {
                Console.Title = Title + ResourcesManager.GetOnlinePlayers().Count.ToString() + "/200";
            }
        }

        public static void TitleD()
        {
            if (Constants.IsPremiumServer)
            {
                Console.Title = Title + ResourcesManager.GetOnlinePlayers().Count.ToString();
            }
            else
            {
                Console.Title = Title + ResourcesManager.GetOnlinePlayers().Count.ToString() + "/200";
            }
        }
    }
}
