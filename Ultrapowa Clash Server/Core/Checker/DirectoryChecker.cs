using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UCS.Core.Checker
{
    internal class DirectoryChecker
    {
        public DirectoryChecker()
        {
            Directorys();
            Files();
            LoadFilter();
        }

        public static List<string> badwords = new List<string>();

        public static void LoadFilter()
        {
            if (File.Exists("filter.ucs"))
            {
                StreamReader sr = new StreamReader(@"filter.ucs");
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    badwords.Add(line);
                }
            }
        }

        public static void Directorys()
        {
            if(!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            if (!Directory.Exists("Patch"))
            {
                Directory.CreateDirectory("Patch");
            }
            if (!Directory.Exists("Tools"))
            {
                Directory.CreateDirectory("Tools");
            }
            if (!Directory.Exists("Library"))
            {
                Directory.CreateDirectory("Library");
            }
            if (!Directory.Exists("Gamefiles"))
            {
                Directory.CreateDirectory("Gamefiles");
            }
            if (!Directory.Exists("Gamefiles/update"))
            {
                Directory.CreateDirectory("Gamefiles/update");
            }
        }

        public static void Files()
        {
			if (!File.Exists("filter.ucs"))
            {
                using (var s = new StreamWriter("filter.ucs"))
                {
                    s.WriteLine("./savegame");
                }
            }
        }
    }
}
