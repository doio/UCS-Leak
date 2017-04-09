
namespace UCS.Core
{
    using System;
    using UCS.Core.Network.TCP;
    using UCS.Core;

    internal class Resources
    {
        internal static Random Random;
        internal static Gateway Gateway;

        internal static DatabaseManager DatabaseManager;
        internal static Loader Loader;


        internal static void Initialize()
        {
            Resources.Loader = new Loader();
            Resources.Random = new Random();
            Resources.Gateway = new Gateway();
            Resources.DatabaseManager = new DatabaseManager();
        }
    }
}
