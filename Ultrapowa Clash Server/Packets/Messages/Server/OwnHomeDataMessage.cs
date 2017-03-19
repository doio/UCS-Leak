using System;
using UCS.Helpers.List;
using UCS.Logic;
using UCS.Logic.Enums;

namespace UCS.Packets.Messages.Server
{
    // Packet 24101
    internal class OwnHomeDataMessage : Message
    {
        public OwnHomeDataMessage(Device client, Level level) : base(client)
        {
            this.Identifier = 24101;
            this.Player = level;
        }

        public Level Player;

        internal override async void Encode()
        {
            try
            {
                ClientAvatar avatar = this.Player.Avatar;
                ClientHome home = new ClientHome(avatar.UserID);

                home.SetShieldTime(avatar.Shield);
                home.SetProtectionTime(avatar.Guard);
                home.SetHomeJSON(Player.SaveToJSON());

                this.Data.AddInt(0);
                this.Data.AddInt(-1);
                this.Data.AddInt((int)Player.Avatar.Update.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                this.Data.AddRange(home.Encode());
                this.Data.AddRange(await avatar.Encode());
                this.Data.AddInt(this.Device.PlayerState == State.WAR_EMODE ? 1 : 0);
                this.Data.AddInt(0);
                this.Data.AddLong(0);
                this.Data.AddLong(0);
                this.Data.AddLong(0);
            }
            catch (Exception)
            {
            }
        }
    }
}