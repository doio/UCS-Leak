using System;
using System.Configuration;
using System.Reflection;
using UCS.Helpers;

namespace UCS.Core.Settings
{
    internal class Constants
    {
        public static string Version                 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Build                   = "8";
        
        public const bool IsPremiumServer            = true;  // false = max. 200 Online Players; true = unlimited Online Players
        public static bool IsRc4                     = Utils.ParseConfigBoolean("UseRc4");  // false = Pepper Crypto

        public const string RedisAddr                = "127.0.0.1";
        public const int RedisPort                   = 6379;

        public const int CleanInterval               = 5000;
        public static int MaxOnlinePlayers           = Utils.ParseConfigInt("MaxOnlinePlayers");
    }
}
