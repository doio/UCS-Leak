using System;
using UCS.Core;
using UCS.Helpers.List;
using UCS.Logic;
using UCS.Logic.Enums;
using UCS.Packets.Messages.Client;

namespace UCS.Packets.Messages.Server
{
    // Packet 24133
    internal class NpcDataMessage : Message
    {
        public NpcDataMessage(Device client, Level level, AttackNpcMessage cnam) : base(client)
        {
            this.Identifier = 24133;
            this.Player = level;
            this.LevelId = cnam.LevelId;
            this.JsonBase = ObjectManager.NpcLevels[LevelId];
            this.Device.PlayerState = State.IN_BATTLE;
        }

        internal override async void Encode()
        {
            try
            {
                ClientHome ownerHome = new ClientHome(Player.Avatar.UserID)
                {
                    ShieldTime = Player.Avatar.Shield,
                    GuardTime = Player.Avatar.Guard,
                    Village = JsonBase
                };

                this.Data.AddInt(0);
                this.Data.AddInt((int)Player.Avatar.Update.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                this.Data.AddRange(ownerHome.Encode());
                this.Data.AddRange(await this.Player.Avatar.Encode());
                this.Data.AddInt(this.LevelId);
            }
            catch (Exception)
            {
            }
        }

        public string JsonBase;
        public int LevelId;
        public Level Player;
    }
}