using System.Threading;

namespace UFD.Core.Threading
{
    internal class CheckerThread
    {
        public static void Start()
        {
            Thread T = new Thread(() =>
            {
                DirChecker.Check();
            }); T.Start();
        }
    }
}
