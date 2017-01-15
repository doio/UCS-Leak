using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UCS.Core.WebAPI
{
    class WebAPI
    {
        private static HttpListener Listener;
        private static int Port = 80;
        private static string URL = "http://localhost:" + Port;

        public WebAPI()
        {
            try
            {
                Listener = new HttpListener();
                Listener.Prefixes.Add(URL);
                Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                Listener.Start();
            }
            catch(Exception)
            {
            }
        }

        public static void Stop()
        {
            Listener.Stop();
        }

        public static string GetStatisticHTML()
        {
            return null;
        }
    }
}
