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
            this.Player.Tick();
        }

        public Level Player;

        internal override async void Encode()
        {
            try
            {
                var _Home =
                    new ClientHome(this.Player.Avatar.UserId)
                    {
                        m_vShieldTime = this.Player.Avatar.m_vShieldTime,
                        m_vProtectionTime = this.Player.Avatar.m_vProtectionTime,
                        Village = this.Player.SaveToJSON()
                    };

                this.Data.AddInt(0);
                this.Data.AddInt(-1);
                this.Data.AddInt((int)Player.Avatar.LastTickSaved.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                this.Data.AddRange(_Home.Encode());
                this.Data.AddRange(await this.Player.Avatar.Encode());
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