using System;
using System.Configuration;
using System.Reflection;
using UCS.Helpers;

namespace UCS.Core.Settings
{
    internal class Constants
    {
        public static string Version                 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Build                   = "12";
        
        public static bool IsRc4                     = Utils.ParseConfigBoolean("UseRc4");  // false = Pepper Crypto

        public const string RedisAddr                = "127.0.0.1";
        public const int RedisPort                   = 6379;

        public const int CleanInterval               = 5000;
        public static int MaxOnlinePlayers           = Utils.ParseConfigInt("MaxOnlinePlayers");
        public static int LicensePlanID              = 1;
    }
}
