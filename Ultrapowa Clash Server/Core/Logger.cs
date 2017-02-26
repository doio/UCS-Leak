using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS.Logic.Enums;
using static System.Convert;

namespace UCS.Core
{
    internal class Logger
    {
        static bool ValidLogLevel;
        static int getlevel = ToInt32(ConfigurationManager.AppSettings["LogLevel"]);
        static string timestamp = Convert.ToString(DateTime.Today).Remove(10).Replace(".", "-").Replace("/", "-");
        static string path = "Logs/log_" + timestamp + "_.txt";
        static SemaphoreSlim _fileLock = new SemaphoreSlim(1);

        public static async void Write(string text)
        {
            if (getlevel != 0)
            {
                try
                {
                    await _fileLock.WaitAsync();
                    if (getlevel == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("[LOG]    " + text);
                        Console.ResetColor();
                    }
                    using (StreamWriter sw = new StreamWriter(path,true))
                    await sw.WriteLineAsync("[LOG]    " + text + " at " + DateTime.UtcNow);
                }
                finally
                {
                    _fileLock.Release();
                }
            }
        }

        public static async void WriteError(string text)
        {
            if (getlevel != 0)
            {
                try
                {
                    await _fileLock.WaitAsync();
                    if (getlevel == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[LOG]    " + text);
                        Console.ResetColor();
                    }
                    using (StreamWriter sw = new StreamWriter(path, true))
                        await sw.WriteLineAsync("[LOG]    " + text + " at " + DateTime.UtcNow);
                }
                finally
                {
                    _fileLock.Release();
                }
            }
        }

        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

        public static void Say(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[UCS]    ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public static void Say()
        {
            Console.WriteLine();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]  " + message);
            Console.ResetColor();
        }

        public Logger()
        {
            if(getlevel > 2)
            {
                ValidLogLevel = false;
                LogLevelError();
            }
            else
            {
                ValidLogLevel = true;
            }
            
            if (getlevel != 0 || ValidLogLevel == true)
            {
                if (!File.Exists("Logs/log_" + timestamp + "_.txt"))
                    using (StreamWriter sw = new StreamWriter("Logs/log_" + timestamp + "_.txt"))
                    {
                        sw.WriteLineAsync("Log file created at " + DateTime.Now);
                        sw.WriteLineAsync();
                    }
            }
        }

        public void LogLevelError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Please choose a valid Log Level");
            Console.WriteLine("UCS Emulator is now closing...");
            Console.ResetColor();
            Thread.Sleep(5000);
            Environment.Exit(0);
        }
    }
}
