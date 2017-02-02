using System;
using System.IO;
using System.Text;
using UCS.Helpers;
using UCS.Logic;

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

        public override void Process(Level level)
        {
            // Todo's:
            //       - Send Message to User that Login was succesfull.
            //       - Save UserID in ClientAvatar
            //       - Send Message to login into the CoC Acc
        }
    }
}