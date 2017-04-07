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
        internal static int OP                   = 0;
        internal static string Title             = $"Ultrapowa Clash Server v{Constants.Version} - ©Ultrapowa | Online Players: ";
        public static Stopwatch _Stopwatch       = new Stopwatch();
        public static string Version { get; set; }

        internal static void Main()
        {
            int GWL_EXSTYLE = -20;
            int WS_EX_LAYERED = 0x80000;
            uint LWA_ALPHA = 0x2;
            IntPtr Handle = GetConsoleWindow();
            SetWindowLong(Handle, GWL_EXSTYLE, (int)GetWindowLong(Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);

            if (Utils.ParseConfigBoolean("Animation"))
            {

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
                            Thread.Sleep(15);
                        }
                    }
                }).Start();
            }
            else
            {
                SetLayeredWindowAttributes(Handle, 0, 227, LWA_ALPHA);
            }

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

            Say();

            Console.ForegroundColor = ConsoleColor.Blue;
            Logger.WriteCenter(@" ____ ___.__   __                                                  ");
            Logger.WriteCenter(@"|    |   \  |_/  |_____________  ______   ______  _  _______       ");
            Logger.WriteCenter(@"|    |   /  |\   __\_  __ \__  \ \____ \ /  _ \ \/ \/ /\__  \      ");
            Logger.WriteCenter(@"|    |  /|  |_|  |  |  | \// __ \|  |_> >  <_> )     /  / __ \_    ");
            Logger.WriteCenter(@"|______/ |____/__|  |__|  (____  /   __/ \____/ \/\_/  (____  /    ");
            Logger.WriteCenter(@"                               \/|__|                       \/     ");
            Logger.WriteCenter("            ");

            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Logger.WriteCenter("+-------------------------------------------------------+");
            Console.ResetColor();
            Logger.WriteCenter("|This program is made by the Ultrapowa Development Team.|");
            Logger.WriteCenter("|    Ultrapowa is not affiliated to \"Supercell, Oy\".    |");
            Logger.WriteCenter("|        This program is copyrighted worldwide.         |");
            Logger.WriteCenter("|   Visit www.ultrapowa.com daily for News & Updates!   |");
            Console.ForegroundColor = ConsoleColor.Blue;
            Logger.WriteCenter("+-------------------------------------------------------+");
            Console.ResetColor();

            Say();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[UCS]    ");
            Version = VersionChecker.GetVersionString();

            _Stopwatch.Start();

            if (Version == Constants.Version)
            {
                Console.WriteLine($"> UCS is up-to-date: {Constants.Version}");
                Console.ResetColor();
                Say();
                Say("Preparing Server...\n");

                Resources.Initialize();
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
                Console.WriteLine($"> UCS is not up-to-date! New Version: {Version}. Aborting...");
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
