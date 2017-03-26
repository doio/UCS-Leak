using System.Collections.Generic;
using System.Net.Sockets;
using UCS.Core.Settings;

namespace UCS.Core.Network
{
    internal class SocketAsyncEventArgsPool
    {
        internal readonly Stack<SocketAsyncEventArgs> Pool;

        private readonly object Gate = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="SocketAsyncEventArgsPool"/> class.
        /// </summary>
        internal SocketAsyncEventArgsPool()
        {
            this.Pool = new Stack<SocketAsyncEventArgs>(Constants.MaxOnlinePlayers);
        }

        /// <summary>
        ///     Dequeues this instance.
        /// </summary>
        /// <returns>
        ///     <see cref="SocketAsyncEventArgs"/>
        /// </returns>
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

        /// <summary>
        ///     Enqueues the specified item.
        /// </summary>
        /// <param name="Args">
        ///     The <see cref="SocketAsyncEventArgs"/> instance containing the event data.
        /// </param>
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