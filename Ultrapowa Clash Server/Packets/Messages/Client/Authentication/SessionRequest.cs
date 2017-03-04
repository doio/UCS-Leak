using System;
using System.Configuration;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Core.Settings;
using UCS.Helpers;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.Enums;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 10100
    internal class SessionRequest : Message
    {

        public SessionRequest(Device client, Reader reader) : base(client, reader)
        {
            this.Device.PlayerState = State.SESSION;
        }

        public string Hash;
        public int MajorVersion;
        public int MinorVersion;
        public int Protocol;
        public int KeyVersion;
        public int Unknown;
        public int DeviceSo;
        public int Store;

        internal override void Decode()
        {
            this.Protocol = this.Reader.ReadInt32();
            this.KeyVersion = this.Reader.ReadInt32();
            this.MajorVersion = this.Reader.ReadInt32();
            this.Unknown = this.Reader.ReadInt32();
            this.MinorVersion = this.Reader.ReadInt32();
            this.Hash = this.Reader.ReadString();
            this.DeviceSo = this.Reader.ReadInt32();
            this.Store = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            new HandshakeSuccess(Device, this).Send();
            /*if (Constants.IsRc4)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["patchingServer"]))
                {
                    LoginFailedMessage p = new LoginFailedMessage(Device);
                    p.SetErrorCode(7);
                    p.SetResourceFingerprintData(ObjectManager.FingerPrint.SaveToJson());
                    p.SetContentURL(ConfigurationManager.AppSettings["patchingServer"]);
                    p.SetUpdateURL(ConfigurationManager.AppSettings["UpdateUrl"]);
                    Processor.Send(p);
                }
            }
            else*/
        }

    }
}