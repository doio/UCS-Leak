using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UCS.Core.Network
{
	internal class Reader
	{
		public delegate void IncomingReadHandler(Reader read, byte[] data);
        public const int BufferSize = 256;
        private readonly byte[] _buffer = new byte[BufferSize];
		private readonly IncomingReadHandler _readHandler;
		public Socket Socket;

		public Reader(Socket socket, IncomingReadHandler readHandler)
		{
            try
            {
                Socket = socket;
                _readHandler = readHandler;
                Socket.BeginReceive(_buffer, 0, BufferSize, 0, OnReceive, this);
            }
            catch (Exception)
            {
            }
		}

		private void OnReceive(IAsyncResult result)
		{
			try
			{
				SocketError tmp;
				int bytesRead = Socket.EndReceive(result, out tmp);
				if (tmp == SocketError.Success && bytesRead > 0)
				{
					byte[] read = new byte[bytesRead];
					Array.Copy(_buffer, 0, read, 0, bytesRead);
					_readHandler(this, read);
					Socket.BeginReceive(_buffer, 0, BufferSize, 0, OnReceive, this);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
