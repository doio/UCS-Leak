using System.Threading;
using Updater.Core.Checker;

namespace Updater.Core.Threading
{
    internal class CheckerThread
    {
        static Thread T { get; set; }

        internal static void Start()
        {

            T = new Thread(() =>
            {
                ProcessChecker.Check();
            });
            T.Start();
        }
    }
}
