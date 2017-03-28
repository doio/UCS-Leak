using System.Net.Sockets;
using UCS.Logic;
using UCS.Helpers;
using System;
using System.Diagnostics;
using UCS.Core;
using UCS.Core.Crypto;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic.Enums;

namespace UCS.Packets
{
    internal class Device
    {
        internal Socket Socket;
        internal Level Player;
        internal Token Token;
        internal Crypto Keys;

        public Device(Socket so)
        {
            this.Socket = so;
            this.Keys = new Crypto();
            this.SocketHandle = so.Handle;
        }
        public Device(Socket so, Token token)
        {
            this.Socket = so;
            this.Keys = new Crypto();
            this.Token = token;
            this.SocketHandle = so.Handle;
        }


        internal State PlayerState = Logic.Enums.State.DISCONNECTED;

        internal IntPtr SocketHandle;

        internal string Interface;
        internal string AndroidID;
        internal string OpenUDID;
        internal string Model;
        internal string OSVersion;
        internal string MACAddress;
        internal string AdvertiseID;
        internal string VendorID;
        internal string IPAddress;

        internal uint ClientSeed;

        internal bool Connected => this.Socket.Connected && (!this.Socket.Poll(1000, SelectMode.SelectRead) || this.Socket.Available != 0);

        public bool IsClientSocketConnected()
        {
            try
            {
                return !((Socket.Poll(1000, SelectMode.SelectRead) && (Socket.Available == 0)) || !Socket.Connected);
            }
            catch
            {
                return false;
            }
        }

        internal void Process(byte[] Buffer)
        {
            if (Buffer.Length >= 7)
            {
                using (Reader Reader = new Reader(Buffer))
                {
                    ushort Identifier = Reader.ReadUInt16();
                    Reader.Seek(1);
                    ushort Length = Reader.ReadUInt16();
                    ushort Version = Reader.ReadUInt16();
                    if (Buffer.Length - 7 >= Length)
                    {
                        if (MessageFactory.Messages.ContainsKey(Identifier))
                        {
                            Message message =  Activator.CreateInstance(MessageFactory.Messages[Identifier], this, Reader) as Message;

                            message.Identifier = Identifier;
                            message.Length = Length;
                            message.Version = Version;

                            message.Reader = Reader;

                            try
                            {
                                Logger.Write("Message " + message.GetType().Name + " is handled");
                                message.Decrypt();
                                message.Decode();
                                message.Process();
                            }
                            catch (Exception Exception)
                            {
                            }
                        }
                        else
                        {
                            Logger.Write("Message " + Identifier + " is unhandled");
                            this.Keys.SNonce.Increment();
                        }

                        if ((Buffer.Length - 7) - Length >= 7)
                        {
                            this.Process(Reader.ReadBytes((Buffer.Length - 7) - Length));
                        }
                        else
                        {
                            this.Token.Reset();
                        }
                    }
                }
            }
        }
    }
}
