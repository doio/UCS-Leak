using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultrapowa_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client _Client = new Client();
            _Client.Connect("gamea.clashofclans.com", 9339);
        }
    }
}
