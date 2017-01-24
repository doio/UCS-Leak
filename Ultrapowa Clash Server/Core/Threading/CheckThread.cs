using System.Threading;
using UCS.Core.Checker;
using UCS.Core.Web;

namespace UCS.Core.Threading
{
    class CheckThread
    {
        public CheckThread()
        {
            new Thread(() =>
            {
                //LicenseChecker.CheckForSavedKey(); //disabled atm
                new VersionChecker();
                new DirectoryChecker(); 
                new Logger();
            }).Start();  // Is this Thread really needed?!?
        }
    }
}
