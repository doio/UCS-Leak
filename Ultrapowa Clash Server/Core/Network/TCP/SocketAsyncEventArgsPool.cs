using System.Collections.Generic;
using System.Net.Sockets;
using UCS.Core.Settings;

namespace UCS.Core.Network
{
    internal class SocketAsyncEventArgsPool
    {
        internal readonly Stack<SocketAsyncEventArgs> Pool;

        private readonly object Gate = new object();

        internal SocketAsyncEventArgsPool()
        {
            this.Pool = new Stack<SocketAsyncEventArgs>(Constants.MaxOnlinePlayers);
        }

        internal SocketAsyncEventArgs Dequeue()
        {
            lock (this.Gate)
            {
                if (this.Pool.Count > 0)
                {
                    return this.Pool.Pop();
                }

                return null;
            }
        }

        internal void Enqueue(SocketAsyncEventArgs Args)
        {
            lock (this.Gate)
            {
                if (this.Pool.Count < Constants.MaxOnlinePlayers + 1)
                {
                    this.Pool.Push(Args);
                }
            }
        }
    }
}