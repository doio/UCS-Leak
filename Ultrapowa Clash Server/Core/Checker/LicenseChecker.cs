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
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
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
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("94.23.23.117"), 8008);

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();
                    if (Connected)
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
                                            break;
                                        }

                                    case 2:
                                        {
                                            Say("UCS is running on Plan (Pro).");
                                            break;
                                        }

                                    case 3:
                                        {
                                            Say("UCS is running on Plan (Ultra).");
                                            break;
                                        }
                                }
                            }
                            else if (id == 100)
                            {
                                Say();
                                Say("This Key has been disabled, please contact the Support at Ultrapowa.com.");
                                Say("UCS will be closed now...");
                                Thread.Sleep(4000);
                                Environment.Exit(0);
                            }
                            else if (id == 200)
                            {
                                Say();
                                Say("This Key is expired, please contact the Support at Ultrapowa.com.");
                                Say("UCS will be closed now...");
                                Thread.Sleep(4000);
                                Environment.Exit(0);
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
                    }
                    else
                    {
                        Error("UCS License server is currently unavailable.");
                        Error("UCS will  running on Plan (Lite).");
                        Constants.LicensePlanID = 1;
                        Program.UpdateTitle();
                    }
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

        private static void Send(Socket client, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
 
            client.BeginSend(byteData, 0, byteData.Length, 0,  new AsyncCallback(SendCallback), client);
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
                    response = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    receiveDone.Set();
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
