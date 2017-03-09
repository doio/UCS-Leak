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
using UCS.Helpers.List;
using UCS.Utilities.ZLib;

namespace UCS.Core.Checker
{
    internal class LicenseChecker
    {
        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 256;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }

        private static String response = String.Empty;
        private static Boolean Connected = false;
        private static Boolean Keep = true;
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public LicenseChecker()
        {
            try
            {
                Program._Stopwatch.Stop();
                
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("94.23.23.117"), 8008);
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                if (Connected)
                {
                    back:
                    string Key = GetKey();
                    if (Key == null)
                    {
                        Send(client, "NEW");

                        sendDone.WaitOne();

                        // Receive the response from the remote device.  
                        Receive(client);
                        receiveDone.WaitOne();
                        if (response.Length == 32)
                        {
                            Say("UCS is running on Plan (Lite).");
                            Say("UCS Key : " + response);
                            CheckIfKeyIsSaved(response);
                            Constants.LicensePlanID = 1;
                            Program.UpdateTitle();
                        }
                        else
                        {
                            Error("Server responded with an invalid key.\nPlease try again.");
                            goto back;
                        }

                    }
                    else if (Key.Length == 32)
                    {

                        Send(client, Key);

                        sendDone.WaitOne();

                        // Receive the response from the remote device.  
                        Receive(client);
                        receiveDone.WaitOne();

                        int id = Convert.ToInt32(response);
                        if (id > 0)
                        {
                            if (id < 4)
                            {
                                Constants.LicensePlanID = id;
                                Program.UpdateTitle();

                                switch (id)
                                {

                                    case 1:
                                        {
                                            Say("UCS is running on Plan (Lite).");
                                            Say("UCS key : " + Key);
                                            break;
                                        }

                                    case 2:
                                        {
                                            Say("UCS is running on Plan (Pro).");
                                            Say("UCS key : " + Key);
                                            break;
                                        }

                                    case 3:
                                        {
                                            Say("UCS is running on Plan (Ultra).");
                                            Say("UCS key : " + Key);
                                            break;
                                        }
                                }
                            }
                            else if (id == 100)
                            {
                                Say();
                                Say("This Key has been disabled, please contact us at ultrapowa.com/forum.");
                                Say("UCS will be closed ...");
                                Thread.Sleep(4000);
                                Environment.Exit(0);
                            }
                            else if (id == 200)
                            {
                                Say();
                                Say("This Key is expired, please contact us at ultrapowa.com/forum.");
                                Say("UCS will be closed ...");
                                DeleteKey(); 
                                Thread.Sleep(4000);
                                Environment.Exit(0);
                            }
                            else if (id == 300)
                            {
                                Say();
                                Say("This Key does not exist.");
                                Keep = false;
                                Say("UCS will be closed ...");
                                DeleteKey();
                                Thread.Sleep(4000);
                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            Say();
                            Error("Invalid response from server.");
                            Say("UCS will proceed to run on Plan (Lite).");
                            Constants.LicensePlanID = 1;
                            Program.UpdateTitle();
                        }
                    }
                    else
                    {
                        Error("Invalid key length. Please make sure key is entered correctly and 32 in length.");
                        goto back;
                    }
                }
                else
                {
                    Error("The UCS License server is currently unavailable.");
                    Say("UCS will proceed to run on Plan (Lite).");
                    Constants.LicensePlanID = 1;
                    Program.UpdateTitle();
                }

                Program._Stopwatch.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Say("UCS will be closed ...");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
        }

        private static void SaveKey(string _Key)
        {
            try
            {
                string _FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Ky01.lic";

                if (File.Exists(_FilePath))
                {
                    File.Delete(_FilePath);

                    using (StreamWriter _SW = new StreamWriter(_FilePath))
                    {
                        _SW.Write(ToHexString(_Key));
                    }
                }
                else
                {
                    using (StreamWriter _SW = new StreamWriter(_FilePath))
                    {
                        _SW.Write(ToHexString(_Key));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void DeleteKey()
        {
            string _FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Ky01.lic";

            try
            {
                if (File.Exists(_FilePath))
                {
                    File.Delete(_FilePath);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void CheckIfKeyIsSaved(string _Key)
        {
            string _FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Ky01.lic";

            if (!File.Exists(_FilePath))
            {
                if (_Key.Length == 32)
                {
                    SaveKey(_Key);
                }
            }
        }

        private static void Send(Socket client, String data)
        {
            List<byte> Data = new List<byte>();
            Data.AddCompressed(data, false);
            client.BeginSend(Data.ToArray(), 0, Data.ToArray().Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {

                    using (var br = new BinaryReader(new MemoryStream(state.buffer)))
                    {

                        byte[] compressed = br.ReadBytes(br.ReadInt32());
                        using (var bra = new BinaryReader(new MemoryStream(compressed)))
                        {
                            var decompressedLength = bra.ReadInt32();
                            var compressedEvent = bra.ReadBytes(decompressedLength);
                            response = ZlibStream.UncompressString(compressedEvent);
                        }
                    }

                    receiveDone.Set();
                    if (response.Length == 32)
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        client.Dispose();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Connected = true;
                connectDone.Set();
            }
            catch (Exception e)
            {
                connectDone.Set();
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
                Say("Type in 'y' to use UCS in Lite mode or 'n' to enter a license key.");
                var a = Console.ReadKey(true);
                if (a.Key == ConsoleKey.Y)
                {
                    return null;
                }
                else
                {
                    Say("Enter your License Key: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[UCS]    ");
                    string Key = Console.ReadLine();
                    SaveKey(Key);
                    return Key;
                }
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
