using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ultrapowa_Client
{
    internal class Program
    {
        public static Stopwatch _Stopwatch = new Stopwatch();

        internal static void Main(string[] args)
        {
            Console.Title = "Ultrapowa Clash Client | 0.0.1";

            int GWL_EXSTYLE = -20;
            int WS_EX_LAYERED = 0x80000;
            uint LWA_ALPHA = 0x2;
            IntPtr Handle = GetConsoleWindow();
            SetWindowLong(Handle, GWL_EXSTYLE, (int)GetWindowLong(Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
            SetLayeredWindowAttributes(Handle, 0, 227, LWA_ALPHA);

            Client _Client = new Client();
            _Client.Connect("gamea.clashofclans.com", 9339);
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
