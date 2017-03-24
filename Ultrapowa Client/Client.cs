using System.Net;
using System.Net.Sockets;
using Sodium;
using System;

namespace Ultrapowa_Client
{
    class Client
    {
        public static Socket _Socket = null;
        internal int _Buffer         = 2048;
        internal KeyPair _ClientKey  = PublicKeyBox.GenerateKeyPair(Utilities.HexToBinary("4102C28189897A48CEDFA8C6E5378F55624F9E8408FA8A376643DBBCE715B21A")); // 8.709.23 Key
        internal KeyPair _ServerKey  = PublicKeyBox.GenerateKeyPair(Utilities.HexToBinary("1891D401FADB51D25D3A9174D472A9F691A45B974285D47729C45C6538070D85"));

        public Client()
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string _HostName, int _Port)
        {
            State _State = new State();

            IPHostEntry ipHostInfo = Dns.GetHostEntry(_HostName);
            IPAddress _IpAddress = ipHostInfo.AddressList[0];
            IPEndPoint _IPEndPoint = new IPEndPoint(_IpAddress, _Port);
            _Socket.Connect(_IPEndPoint);
            _Socket.BeginReceive(new byte[this._Buffer], 0, this._Buffer, 0, Callback, _State);
        }

        private void Callback(IAsyncResult _AI)
        {
            //...
        }
    }
}
