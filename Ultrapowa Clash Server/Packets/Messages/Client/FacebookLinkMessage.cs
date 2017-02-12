using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14201
    internal class FacebookLinkMessage : Message
    {
        public FacebookLinkMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                Unknown = br.ReadBoolean(); // Unknown, maybe if Logged in True if no False
                UserID = br.ReadString(); // Facebook UserID (https://www.facebook.com/ + UserID)
            }
        }

        public string UserID { get; set; }

        public bool Unknown { get; set; }

        public override async void Process(Level level)
        {
            try
            {
                ClientAvatar player = level.GetPlayerAvatar();

                if (ResourcesManager.GetPlayerWithFacebookID(UserID) != null)
                {
                    Level l = await ResourcesManager.GetPlayerWithFacebookID(UserID);
                    PacketProcessor.Send(new OwnHomeDataMessage(Client, l)); // Not done
                    PacketProcessor.Send(new OutOfSyncMessage(l.GetClient()));
                }
                else
                {
                    player.SetFacebookID(UserID);
                    PacketProcessor.Send(new OutOfSyncMessage(Client));
                }
            } catch (Exception) { }
        }
    }
}