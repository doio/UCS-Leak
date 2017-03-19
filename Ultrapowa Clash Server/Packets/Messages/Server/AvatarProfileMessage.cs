using System;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24334
    internal class AvatarProfileMessage : Message
    {
        internal Level Level;

        public AvatarProfileMessage(Device client) : base(client)
        {
            this.Identifier = 24334;
        }

        internal override async void Encode()
        {
            try
            {
                ClientHome ch = new ClientHome(this.Level.Avatar.UserID);
                ch.SetHomeJSON(this.Level.SaveToJSON());

                this.Data.AddRange(await this.Level.Avatar.Encode());
                this.Data.AddCompressed(ch.GetHomeJSON(), false);

                this.Data.AddInt(this.Level.Avatar.Donations); //Donated
                this.Data.AddInt(this.Level.Avatar.Received); //Received
                this.Data.AddInt(0); //War Cooldown

                this.Data.AddInt(0); //Unknown
                this.Data.Add(0); //Unknown
            }
            catch (Exception)
            {
            }
        }
    }
}