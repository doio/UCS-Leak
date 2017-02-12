using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using UCS.Core.Threading;
using UCS.Core.Web;
using static System.Console;
using static UCS.Core.Logger;

namespace UCS.Core.Settings
{
    internal class UCSControl
    {
        public static void UCSClose()
        {
            new Thread(() =>
            {
                Say("Closing UCS...");
                Environment.Exit(0);
            }).Start();
        }

        public static void UCSRestart()
        {
            new Thread(() =>
            {
                Say("Restarting UCS...");
                Thread.Sleep(200);
                Process.Start("UCS.exe");
                Environment.Exit(0);
            }).Start();
        }
    }
}
