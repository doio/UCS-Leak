using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UCS.Core.Settings;
using UCS.Packets;

namespace UCS.Core.Network
{
    internal class Token
    {
        internal Device Device;
        internal SocketAsyncEventArgs Args;
        internal List<byte> Packet;

        internal byte[] Buffer;

        internal int Offset;

        internal bool Aborting;

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="Socket">The socket.</param>
        internal Token(SocketAsyncEventArgs Args, Device Device)
        {
            this.Device = Device;
            this.Device.Token = this;

            this.Args = Args;
            this.Args.UserToken = this;

            this.Buffer = new byte[Constants.ReceiveBuffer];
            this.Packet = new List<byte>(Constants.ReceiveBuffer);
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        internal void SetData()
        {
            byte[] Data = new byte[this.Args.BytesTransferred];
            Array.Copy(this.Args.Buffer, 0, Data, 0, this.Args.BytesTransferred);
            this.Packet.AddRange(Data);
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        internal void Process()
        {
            byte[] Data = this.Packet.ToArray();
            this.Device.Process(Data);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        internal void Reset()
        {
            this.Offset = 0;
            this.Packet.Clear();
        }
    }
}