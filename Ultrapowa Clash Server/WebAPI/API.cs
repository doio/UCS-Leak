using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS.Core;

namespace UCS.WebAPI
{
    class API
    {
        private static IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        private static HttpListener Listener;
        private static int Port = 88; // TODO: Add it to the config File
        private static string IP = ipHostInfo.AddressList[0].ToString();
        private static string URL = "http://" + IP + ":" + Port + "/";

        public static string HTML()
        {
            try
            {
                using (StreamReader sr = new StreamReader("WebAPI/HTML/Statistics.html"))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                Console.WriteLine();
                return "File not Found";
            }
        }

        public API()
        {
            try
            {
                if(!HttpListener.IsSupported)
                {
                    Logger.Say("The current System doesn't support the WebAPI.");
                    return;
                }

                Listener = new HttpListener();
                Listener.Prefixes.Add(URL);
                Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                Listener.Start();

                Console.WriteLine("[UCS]    The WebAPI has been started on '" + Port + "'");

                ThreadPool.QueueUserWorkItem((o) =>
                {
                    while (Listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            HttpListenerContext ctx = (HttpListenerContext)c;
                            byte[] responseBuf = Encoding.UTF8.GetBytes(GetStatisticHTML());
                            ctx.Response.ContentLength64 = responseBuf.Length;
                            ctx.Response.OutputStream.Write(responseBuf, 0, responseBuf.Length);
                            ctx.Response.OutputStream.Close();

                        }, Listener.GetContext());
                    }
                });
            }
            catch(Exception)
            {
                Console.WriteLine("[UCS]    Please check if the Port '" + Port + "' is not in use.");
            }
        }

        public static void Stop()
        {
            Listener.Stop();
        }

        public static string GetStatisticHTML()
        {
            try
            {
                return HTML()
                    .Replace("%ONLINEPLAYERS%", ResourcesManager.GetOnlinePlayers().Count.ToString())
                    .Replace("%INMEMORYPLAYERS%", ResourcesManager.GetInMemoryLevels().Count.ToString())
                    .Replace("%INMEMORYALLIANCES%", ResourcesManager.GetInMemoryAlliances().Count.ToString())
                    .Replace("%TOTALCONNECTIONS%", ResourcesManager.GetConnectedClients().Count.ToString());
            }
            catch(Exception)
            {
                return "The server encountered an internal error or misconfiguration and was unable to complete your request. (500)";
            }
        }
    }
}
