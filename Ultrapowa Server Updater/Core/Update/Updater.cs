using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Updater.Core.Update
{
    class Updater
    {
        public static void DownloadUpdate()
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://ucs-up.000webhostapp.com/UCS_Update.zip", @"..\Update.zip");
                Say("UCS Update Downloaded Successfully!");
                Unzip();
            }
            catch (Exception)
            {
                Error("Problem with downloading the UCS update, check your Network");
                StartUCS();
            }
        }

        public static void Unzip()
        {
            if (Directory.Exists(@"..\Update"))
            {
                Directory.Delete(@"..\Update", true);
                Directory.CreateDirectory(@"..\Update");
            }
            else
            {
                Directory.CreateDirectory(@"..\Update");
            }
            try
            {
                string TargetDirectory = @"..\Update/";
                using (ZipFile zip = ZipFile.Read(@"..\Update.zip"))
                    zip.ExtractAll(TargetDirectory, ExtractExistingFileAction.Throw);
                File.Delete(@"..\Update.zip");

                Say("UCS Update Unzipped!");

                Update();

                //Delete Work Path
                Directory.Delete(@"..\Update", true);

                Say("UCS Update Installed Successfully, Starting UCS ...");
                StartUCS();
            }
            catch (Exception)
            {
                Error("Problem with Unizp and Delete");
            }
        }

        public static void Update()
        {
            var directory = new DirectoryInfo(@"..\Update");
            var files = directory.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string destination = file.FullName.Replace(directory.FullName + @"\", "..\\");

                Console.WriteLine("[USU]    Installing file {0} ...", destination);
                Directory.CreateDirectory(new FileInfo(destination).DirectoryName);
                file.CopyTo(destination, true);
            }
        }

        public static void Say(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("[USU]    " + msg);
            Console.ResetColor();
        }

        public static void Error(string err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]  " + err);
            Console.ResetColor();
        }

        public static void StartUCS()
        {
            Process.Start(@"..\UCS.exe");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}
