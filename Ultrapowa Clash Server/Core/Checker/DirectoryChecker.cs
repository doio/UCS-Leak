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
            LoadEvents();
            Directorys();
            Files();
            LoadFilter();
        }

        public static List<string> badwords = new List<string>();

        public static string _Events = null;

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

        public static void LoadEvents()
        {
            if (File.Exists("calendar.json"))
            {
                _Events = File.ReadAllText("calendar.json");
            }
            else
            {
                _Events = ("{\"event\":[]}");
            }
        }

        public static void Directorys()
        {
            string[] Dirs = 
            {
                "Logs",
                "Patch",
                "Tools",
                "Library",
                "Gamefiles",
                "Gamefiles/update"
            };

            foreach (string Dir in Dirs)
            {
                if (!Directory.Exists(@Dir))
                {
                    Directory.CreateDirectory(@Dir);
                }
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
