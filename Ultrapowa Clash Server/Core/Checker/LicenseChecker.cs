using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using static UCS.Core.Logger;

namespace UCS.Core
{
    internal class LicenseChecker
    {
        public static bool PremiumServer;

        private static string User;

        private static void CheckForPremium(string name, string key)
        {
            try
            {
                var w = new WebClient();
                var Key = w.DownloadString("http://clashoflights.cf/UCS/Key_" + name + "_.license");
                if (Key == null)
                {
                    DisablePremiumFeatures();
                }
                else if (Key == key)
                {
                    ActivatePremiumFeatures();
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/UCS." + name + ".license";
                    using (var s = new StreamWriter(path))
                    {
                        s.WriteLine(ToHexString(key));
                    }
                    Say();
                    Say("Activation succeded!");
                    Say("Restarting UCS to save all changes... ");
                    Thread.Sleep(5000);
                    Process.Start("UCS.exe");
                    Environment.Exit(0);
                }
                else
                {
                    DisablePremiumFeatures();
                    Say();
                    Error("User/Key is not valid!");
                    Say();
                    Error("UCS will be closed now...");
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }
            }
            catch(Exception)
            {
                Say();
                Error("User/Key is not valid!");
                Say();
                Error("UCS will be closed now...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
        }

        private static void ActivatePremiumFeatures()
        {
            PremiumServer = true;
        }

        private static void DisablePremiumFeatures()
        {
            PremiumServer = false;
        }

        public static void ActivateUCS()
        {
            Say();
            Say("To continue you have to activate UCS with your User ID and Activation Key.");
            Say("Type in now your Username: ");
            User = Console.ReadLine();
            Say("Type in now your Activation Key: ");
            var ActivationKey = Console.ReadLine();
            Say("Trying to activate UCS...");
            if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data");
                var s = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license");
                s.WriteLine(ToHexString(User));
                s.Close();
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license");
                    var s = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license");
                    s.WriteLine(ToHexString(User));
                    s.Close();
                }
                else
                {
                    var s = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license");
                    s.WriteLine(ToHexString(User));
                    s.Close();
                }
            }
            CheckForPremium(User, ActivationKey);
        }

        public static void CheckForSavedKey()
        {
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license"))
                {
                    ActivateUCS();
                }

                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Data/Data.license";
                var r = new StreamReader(path);
                var UName = FromHexString(r.ReadLine());
                User = UName;
                r.Close();

                var w = new WebClient();
                var Key = w.DownloadString("http://clashoflights.cf/UCS/Key_" + User + "_.license");
                string key;
                var file = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/UCS." + User + ".license";
                var gkey = new StreamReader(file);

                key = FromHexString(gkey.ReadLine());
                gkey.Close();

                if (!File.Exists(file))
                {
                    ActivateUCS();
                }
                else
                {
                    if (key == Key)
                    {
                        Say();
                        Say("You are using a Licensed version of UCS.");
                    }
                    else
                    {
                        ActivateUCS();
                    }
                }
            }
            catch (Exception)
            {
                Say();
                Error("[UCS]    Error [404] Key not found!");
                ActivateUCS();
            }
        }

        private static string ToHexString(string str)
        {
            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        private static string FromHexString(string String)
        {
            var bytes = new byte[String.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(String.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
