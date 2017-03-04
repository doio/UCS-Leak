using System;
using UCS.Helpers;
using UCS.Packets;

namespace UCS.Core.Network
{
    internal static class Processor
    {
        /// <summary>
        /// Recepts the specified message.
        /// </summary>
        /// <param name="Message">The message.</param>
        internal static void Recept(this Message Message)
        {
            Message.Decrypt();
            Message.Decode();
            Message.Process();
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="Message">The message.</param>
        internal static void Send(this Message Message)
        {
            try
            {
                Message.Encode();
                Message.Encrypt();
#if DEBUG
                if (Message.Device.Connected)
                {
                    System.Diagnostics.Debug.WriteLine(Utils.Padding(Message.Device.Socket.RemoteEndPoint.ToString(), 15) + " <-- " + Message.GetType().Name);
                }
#endif
                Resources.Gateway.Send(Message);

                Message.Process();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="Command">The command.</param>
        /// <returns></returns>
        internal static Command Handle(this Command Command)
        {
            Command.Encode();

            return Command;
        }
    }
}