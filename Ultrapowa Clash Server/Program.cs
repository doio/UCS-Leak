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
    internal class Program
    {
        public static int OP                   = 0;
        public static string Title             = "Ultrapowa Clash Server v" + Constants.Version + " - ©Ultrapowa | Online Players: ";
        public static Stopwatch _Stopwatch     = new Stopwatch();
        public static string Version { get; set; }

        private static void Main()
        {
            if (Utils.ParseConfigBoolean("Animation"))
            {
                int GWL_EXSTYLE = -20;
                int WS_EX_LAYERED = 0x80000;
                uint LWA_ALPHA = 0x2;
                IntPtr Handle = GetConsoleWindow();
                SetWindowLong(Handle, GWL_EXSTYLE, (int)GetWindowLong(Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
                SetLayeredWindowAttributes(Handle, 0, 20, LWA_ALPHA);

                new Thread(() =>
                {
                    for (int i = 20; i < 227; i++)
                    {
                        if (i < 100)
                        {
                            SetLayeredWindowAttributes(Handle, 0, (byte)i, LWA_ALPHA);
                            Thread.Sleep(5);
                        }
                        else
                        {
                            SetLayeredWindowAttributes(Handle, 0, (byte)i, LWA_ALPHA);
                            Thread.Sleep(10);
                        }
                    }
                }).Start();
            }

            switch (Constants.LicensePlanID)
            {
                case 3:
                    Console.Title = Title + OP;
                    break;
                case 2:
                    Console.Title = Title + OP + "/700";
                    break;
                case 1:
                    Console.Title = Title + OP + "/350";
                    break;
            }

            Say();

            Console.ForegroundColor = ConsoleColor.Blue;
            Logger.WriteCenter(@" ____ ___.__   __                                                  ");
            Logger.WriteCenter(@"|    |   \  |_/  |_____________  ______   ______  _  _______       ");
            Logger.WriteCenter(@"|    |   /  |\   __\_  __ \__  \ \____ \ /  _ \ \/ \/ /\__  \      ");
            Logger.WriteCenter(@"|    |  /|  |_|  |  |  | \// __ \|  |_> >  <_> )     /  / __ \_    ");
            Logger.WriteCenter(@"|______/ |____/__|  |__|  (____  /   __/ \____/ \/\_/  (____  /    ");
            Logger.WriteCenter(@"                               \/|__|                       \/     ");
            Logger.WriteCenter("            ");
            Logger.WriteCenter("+-------------------------------------------------------+");
            Logger.WriteCenter("|This program is made by the Ultrapowa Development Team.|");
            Logger.WriteCenter("|    Ultrapowa is not affiliated to \"Supercell, Oy\".    |");
            Logger.WriteCenter("|        This program is copyrighted worldwide.         |");
            Logger.WriteCenter("|   Visit www.ultrapowa.com daily for News & Updates!   |");
            Logger.WriteCenter("+-------------------------------------------------------+");
            Console.ResetColor();

            Say();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[UCS]    ");
            Version = VersionChecker.GetVersionString();

            _Stopwatch.Start();

            if (Version == Constants.Version)
            {
                Console.WriteLine("> UCS is up-to-date: " + Constants.Version);
                Console.ResetColor();
                Say("By downloading or using this software, you accept the terms of the software license agreement.");
                Say();
                Say("Preparing Server...\n");

                Resources.Initialize();
            }
            else if (Version == "Error")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> An Error occured when requesting the Version number.");
                Console.WriteLine();
                Logger.Say("Please contact us for support at https://ultrapowa.com/forum!");
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
            switch (Constants.LicensePlanID)
            {
                case 3:
                    Console.Title = Title + OP;
                    break;
                case 2:
                    Console.Title = Title + OP + "/700";
                    break;
                case 1:
                    Console.Title = Title + OP + "/350";
                    break;
            }
        }

        public static void TitleU()
        {
            switch (Constants.LicensePlanID)
            {
                case 3:
                    Console.Title = Title + ++OP;
                    break;
                case 2:
                    Console.Title = Title + ++OP + "/700";
                    break;
                case 1:
                    Console.Title = Title + ++OP + "/350";
                    break;
            }
        }

        public static void TitleD()
        {
            switch (Constants.LicensePlanID)
            {
                case 3:
                    Console.Title = Title + --OP;
                    break;
                case 2:
                    Console.Title = Title + --OP + "/700";
                    break;
                case 1:
                    Console.Title = Title + --OP + "/350";
                    break;
            }
        }


        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();
    }
}
