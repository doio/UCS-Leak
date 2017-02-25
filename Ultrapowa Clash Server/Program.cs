using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UCS.Core;
using UCS.Core.Checker;
using UCS.Core.Network;
using UCS.Core.Settings;
using UCS.Core.Threading;
using UCS.Core.Web;
using UCS.Helpers;
using UCS.WebAPI;
using static UCS.Core.Logger;

namespace UCS
{
    class Program
    {
        public static int OP                   = 0;
        public static string Title             = "Ultrapowa Clash Server v" + Constants.Version + " - © " + DateTime.Now.Year + " | Online Players: ";
        public static Stopwatch _Stopwatch     = new Stopwatch();
        public static string Version { get; set; }
        private static Loader _Loader          = null;

        static void Main()
        {
            if (Constants.LicensePlanID == 3)
            {
                Console.Title = Title + OP;
            }
            else if(Constants.LicensePlanID == 2)
            {
                Console.Title = Title + OP + "/700";
            }
            else if (Constants.LicensePlanID == 1)
            {
                Console.Title = Title + OP + "/350";
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
            Say("> This program is made by the Ultrapowa Development Team.");
            Say("> Ultrapowa is not affiliated to \"Supercell, Oy\".");
            Say("> This program is copyrighted worldwide.");
            Say("> Visit www.ultrapowa.com daily for News & Updates!");

            if (Constants.IsRc4)
            {
                Say("> UCS is running under RC4 mode. Please make sure CSV is modded to allow RC4.");
            }
            else
            {
                Say("> UCS is running under Pepper mode. Please make sure client key is modded.");
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[UCS]    ");
            Version = VersionChecker.GetVersionString();

            _Stopwatch.Start();

            if (Version == Constants.Version)
            {
                Console.WriteLine("> UCS is up-to-date: " + Constants.Version);
                Console.ResetColor();
                Say();
                Say("Preparing Server...\n");

                new Thread(() =>
                {
                    _Loader = new Loader();
                }).Start();
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
        }

        public static void UpdateTitle()
        {
            if (Constants.LicensePlanID == 3)
            {
                Console.Title = Title + OP;
            }
            else if (Constants.LicensePlanID == 2)
            {
                Console.Title = Title + OP + "/700";
            }
            else if (Constants.LicensePlanID == 1)
            {
                Console.Title = Title + OP + "/350";
            }
        }

        public static void TitleU()
        {
            if (Constants.LicensePlanID == 3)
            {
                Console.Title = Title + ++OP;
            }
            else if(Constants.LicensePlanID == 2)
            {
                Console.Title = Title + ++OP + "/700";
            }
            else if (Constants.LicensePlanID == 1)
            {
                Console.Title = Title + ++OP + "/350";
            }
        }

        public static void TitleD()
        {
            if (Constants.LicensePlanID == 3)
            {
                Console.Title = Title + --OP;
            }
            else if(Constants.LicensePlanID == 2)
            {
                Console.Title = Title + --OP + "/700";
            }
            else if(Constants.LicensePlanID == 1)
            {
                Console.Title = Title + --OP + "/350";
            }
        }
    }
}
