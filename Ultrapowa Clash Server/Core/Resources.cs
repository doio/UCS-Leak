
using UCS.Core.Events;

namespace UCS.Core
{
    using System;
    using UCS.Core.Network.TCP;

    internal class Resources
    {
        internal static Random Random;
        internal static Gateway Gateway;
        internal static DatabaseManager DatabaseManager;
        internal static Loader Loader;
        internal static Region Region;


        internal static void Initialize()
        {
            Resources.Loader = new Loader();
            Resources.Random = new Random();
            Resources.DatabaseManager = new DatabaseManager();
            Resources.Region = new Region();
            Resources.Gateway = new Gateway();
        }
    }
}
