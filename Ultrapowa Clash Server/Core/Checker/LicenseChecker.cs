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
using System.Net.Sockets;
using UCS.Core.Settings;

namespace UCS.Core
{
    class LicenseChecker
    {
        public LicenseChecker()
        {
            try
            {
                Program._Stopwatch.Stop();

                back:
                string Key = GetKey();
                if (Key.Length == 32)
                {
                    CheckIfKeyIsSaved(Key);
                }

                if (Key.Length == 32)
                {
                    TcpClient client     = new TcpClient("94.23.23.117", 8008);
                    byte[] data          = Encoding.ASCII.GetBytes(Key);
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                    data                 = new byte[256];
                    string responseData  = string.Empty;
                    int bytes            = stream.Read(data, 0, data.Length);
                    responseData         = Encoding.ASCII.GetString(data, 0, bytes);

                    if (Convert.ToInt32(responseData) > 0)
                    {
                        if (Convert.ToInt32(responseData) < 4)
                        {
                            Constants.LicensePlanID = Convert.ToInt32(responseData);
                            Program.UpdateTitle();

                            if (Convert.ToInt32(responseData)      == 1)
                            {
                                Say("UCS is running on Plan (Lite).");
                            }
                            else if (Convert.ToInt32(responseData) == 2)
                            {
                                Say("UCS is running on Plan (Pro).");
                            }
                            else if (Convert.ToInt32(responseData) == 3)
                            {
                                Say("UCS is running on Plan (Ultra).");
                            }
                        }
                        else if(Convert.ToInt32(responseData) == 100)
                        {
                            Say();
                            Say("This Key has been disabled, please contact the Support at Ultrapowa.com.");
                            Say("UCS will be closed now...");
                            Thread.Sleep(4000);
                            Environment.Exit(0);
                        }
                        else if (Convert.ToInt32(responseData) == 200)
                        {
                            Say();
                            Say("This Key is expired, please contact the Support at Ultrapowa.com.");
                            Say("UCS will be closed now...");
                            Thread.Sleep(4000);
                            Environment.Exit(0);
                        }
                        else if (Convert.ToInt32(responseData) == 300)
                        {
                            Say();
                            Say("The installed Key is invalid!");
                            try
                            {
                                string _FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Ky01.lic";
                                File.Delete(_FilePath);
                            }
                            catch (Exception)
                            {
                            }
                            goto back;
                        }
                    }
                    else
                    {
                        Say();
                        Say("This Key is not valid.");
                        Say("UCS will be closed now...");
                        Thread.Sleep(4000);
                        Environment.Exit(0);
                    }
                    stream.Close();
                    client.Close();
                }
                else
                {
                    Say("You entered a wrong Key! Please try again.");
                    goto back;
                }

                Program._Stopwatch.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Say("UCS will be closed now...");
                Thread.Sleep(4000);
                Environment.Exit(0);
            }
        }

        private static void CheckIfKeyIsSaved(string _Key)
        {
            string _FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Ky01.lic";
            if (!File.Exists(_FilePath))
            {
                if (_Key.Length == 32)
                {
                    using (StreamWriter _SW = new StreamWriter(_FilePath))
                    {
                        _SW.Write(ToHexString(_Key));
                    }
                }
            }
        }

        private static string GetKey()
        {
            back:
            string _FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Ky01.lic";
            if (File.Exists(_FilePath))
            {
                string Data = FromHexString(File.ReadAllText(_FilePath));
                if (Data.Length == 32)
                {
                    return Data;
                }
                else
                {
                    File.Delete(_FilePath);
                    goto back;
                }
            }
            else
            {
                Say("Enter now your License Key:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("[UCS]    ");
                Console.ResetColor();
                goback:
                string Key = Console.ReadLine();
                return Key;
            }
        }

        private static string ToHexString(string str)
        {
            var sb    = new StringBuilder();
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
