using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UCS.Files;
using UCS.Files.CSV;
using UCS.Files.Logic;
using static UCS.Core.Logger;

namespace UCS.Core
{
    internal class CSVManager
    {
        private static readonly List<Tuple<string, string, int>> _gameFiles = new List<Tuple<string, string, int>>();

        private static DataTables _DataTables;

        public CSVManager()
        {
            DownloadLatestCSVFiles();

            try
            {
                _gameFiles.Add(new Tuple<string, string, int>("Buildings", @"Gamefiles/logic/buildings.csv", 0));
                _gameFiles.Add(new Tuple<string, string, int>("Resources", @"Gamefiles/logic/resources.csv", 2));
                _gameFiles.Add(new Tuple<string, string, int>("Characters", @"Gamefiles/logic/characters.csv", 3));
                _gameFiles.Add(new Tuple<string, string, int>("Obstacles", @"Gamefiles/logic/obstacles.csv", 7));
                _gameFiles.Add(new Tuple<string, string, int>("Experience Levels", @"Gamefiles/logic/experience_levels.csv", 10));
                _gameFiles.Add(new Tuple<string, string, int>("Traps", @"Gamefiles/logic/traps.csv", 11));
                _gameFiles.Add(new Tuple<string, string, int>("Leagues", @"Gamefiles/logic/leagues.csv", 12));
                _gameFiles.Add(new Tuple<string, string, int>("Globals", @"Gamefiles/logic/globals.csv", 13));
                _gameFiles.Add(new Tuple<string, string, int>("Townhall Levels", @"Gamefiles/logic/townhall_levels.csv", 14));
                _gameFiles.Add(new Tuple<string, string, int>("NPCs", @"Gamefiles/logic/npcs.csv", 16));
                _gameFiles.Add(new Tuple<string, string, int>("Decos", @"Gamefiles/logic/decos.csv", 17));
                _gameFiles.Add(new Tuple<string, string, int>("Shields", @"Gamefiles/logic/shields.csv", 19));
                _gameFiles.Add(new Tuple<string, string, int>("Achievements", @"Gamefiles/logic/achievements.csv", 22));
                _gameFiles.Add(new Tuple<string, string, int>("Spells", @"Gamefiles/logic/spells.csv", 25));
                _gameFiles.Add(new Tuple<string, string, int>("Heroes", @"Gamefiles/logic/heroes.csv", 27));
                /*
                gameFiles.Add(new Tuple<string, string, int>("Alliance Badge Layers", @"Gamefiles/logic/alliance_badge_layers.csv", 30));
                gameFiles.Add(new Tuple<string, string, int>("Alliance Badges", @"Gamefiles/logic/alliance_badges.csv", 31));
                gameFiles.Add(new Tuple<string, string, int>("Alliance Levels", @"Gamefiles/logic/alliance_levels.csv", 32));
                gameFiles.Add(new Tuple<string, string, int>("Alliance Portal", @"Gamefiles/logic/alliance_portal.csv", 33));
                gameFiles.Add(new Tuple<string, string, int>("Buildings Classes", @"Gamefiles/logic/building_classes.csv", 34));
                gameFiles.Add(new Tuple<string, string, int>("Effects", @"Gamefiles/logic/effects.csv", 35));
                gameFiles.Add(new Tuple<string, string, int>("Locales", @"Gamefiles/logic/locales.csv", 36));
                gameFiles.Add(new Tuple<string, string, int>("Missions", @"Gamefiles/logic/missions.csv", 37));
                gameFiles.Add(new Tuple<string, string, int>("Projectiles", @"Gamefiles/logic/projectiles.csv", 38));
                gameFiles.Add(new Tuple<string, string, int>("Regions", @"Gamefiles/logic/regions.csv", 39));
                gameFiles.Add(new Tuple<string, string, int>("Variables", @"Gamefiles/logic/variables.csv", 40)); 
                gameFiles.Add(new Tuple<string, string, int>("War", @"Gamefiles/logic/war.csv", 28));
                */
                _DataTables = new DataTables();
                int Count = 0;
                foreach (var _File in _gameFiles)
                {
                    _DataTables.InitDataTable(new CSVTable(_File.Item2), _File.Item3);
                    Count++;
                }
                Say("CSV Tables  have been succesfully loaded. (" + Count + ")");
            }
            catch (Exception)
            {
                Say();
                Error("Error loading Gamefiles. Looks like you have :");
                Error("     -> Edited the Files Wrong");
                Error("     -> Made mistakes by deleting values");
                Error("     -> Entered too High or Low value");
                Error("     -> Please check to these errors");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        public static List<Tuple<string, string, int>> Gamefiles => _gameFiles;

        public static DataTables DataTables => _DataTables;

        public static void DownloadLatestCSVFiles()
        {
            try
            {
                string _DownloadString = "http://b46f744d64acd2191eda-3720c0374d47e9a0dd52be4d281c260f.r11.cf2.rackcdn.com/" + ObjectManager.FingerPrint.sha + "/";
                
            }
            catch (Exception)
            { 
            }
        } 

        public static void DownloadFileFromWebServer(string _Link, string _Sublink, string _Folder)
        {
            try
            {
                string _FileLink = _Link + _Sublink;

                if (_Folder == "csv")
                {
                    if (!Directory.Exists("Gamefiles/update/csv"))
                    {
                        Directory.CreateDirectory("Gamefiles/update/csv");
                    }
                }
                else if (_Folder == "logic")
                {
                    if(!Directory.Exists("Gamefiles/update/logic"))
                    {
                        Directory.CreateDirectory("Gamefiles/update/logic");
                    }  
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
                                                                                                       