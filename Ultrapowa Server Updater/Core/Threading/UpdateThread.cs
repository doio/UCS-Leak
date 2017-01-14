using System.Threading;

namespace Updater.Core.Threading
{
    internal class UpdateThread
    {
        static Thread T { get; set; }

        internal static void Start()
        {

            T = new Thread(() =>
            {
                Update.Updater.DownloadUpdate();
            });
            T.Start();
        }
    }
}
