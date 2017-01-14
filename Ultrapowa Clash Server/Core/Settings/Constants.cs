using System;
using System.Configuration;
using System.Reflection;
using UCS.Helpers;

namespace UCS.Core.Settings
{
    internal class Constants
    {
        public static string Version                 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
        public const bool IsPremiumServer            = true;  // false = max100 Online Players; true = unlimited Online Players
        public static bool IsRc4                     = Utils.ParseConfigBoolean("UseRc4");  // false = Pepper Crypto

        public const string RedisAddr                = "127.0.0.1";
        public const int RedisPort                   = 6379;
    }
}
