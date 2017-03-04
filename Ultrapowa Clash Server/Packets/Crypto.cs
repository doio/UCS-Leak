using System;

namespace UCS.Packets
{
    internal class Crypto : IDisposable
    {
        internal byte[] SNonce;
        internal byte[] RNonce;
        internal byte[] PublicKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crypto"/> class.
        /// </summary>
        internal Crypto()
        {
            this.PublicKey = new byte[32];
            this.SNonce = new byte[24];
            this.RNonce = new byte[24];
        }

        /// <summary>
        /// Exécute les tâches définies par l'application associées 
        /// à la libération ou à la redéfinition des ressources non managées.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.SNonce = null;
            this.RNonce = null;
            this.PublicKey = null;
        }
    }
}
