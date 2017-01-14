using System;
using System.Reflection;
using System.Threading;
using static System.Console;

namespace Updater.Core.Threading
{
    internal class ConsoleThread
    {
        static Thread T { get; set; }

        internal static void Start()
        {
            T = new Thread(() =>
            {
                string version = "" + Assembly.GetExecutingAssembly().GetName().Version;
                Title = "Ultrapowa Server Updater v" + version + " - © 2016";

                ForegroundColor = ConsoleColor.Red;
                WriteLine(
                    @"
      ____ ___.__   __                                              
     |    |   \  |_/  |_____________  ______   ______  _  _______   
     |    |   /  |\   __\_  __ \__  \ \____ \ /  _ \ \/ \/ /\__  \  
     |    |  /|  |_|  |  |  | \// __ \|  |_> >  <_> )     /  / __ \_
     |______/ |____/__|  |__|  (____  /   __/ \____/ \/\_/  (____  /
                                    \/|__|                       \/
                  ");

                ResetColor();
                WriteLine("[USU]    > This program is made by the Ultrapowa Development Team.\n[USU]    > Ultrapowa is not affiliated to \"Supercell, Oy\".\n[USU]    > USU is proudly licensed under the MIT-License.\n[USU]    > Visit www.ultrapowa.com daily for News & Updates!");

                CheckerThread.Start();
                //UpdateThread.Start();

                ForegroundColor = ConsoleColor.DarkCyan;
                WriteLine("\n[USU]    Updating UCS ...\n");
                ResetColor();
            });
            T.Start();
            ReadLine();
        }
    }
}
