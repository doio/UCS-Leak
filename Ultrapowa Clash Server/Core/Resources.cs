
namespace UCS.Core
{
    using System;
    using UCS.Core.Network;
    internal class Resources
    {
        internal static Random Random;
        internal static Gateway Gateway;
        internal static Loader Loader;


        internal static void Initialize()
        {
            Resources.Loader = new Loader();
            Resources.Random = new Random();
            Resources.Gateway = new Gateway();
        }
    }
}
