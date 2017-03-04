using System;
using UCS.Core;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24133
    internal class RetributionDataMessage : Message
    {
        public RetributionDataMessage(Device client, Level level, int id) : base(client)
        {
            this.Identifier = 24133; // New one needed
            this.Player = level;
            this.LevelId = id;
            this.JsonBase = ObjectManager.NpcLevels[LevelId];
            this.Device.PlayerState = Logic.Enums.State.IN_BATTLE;
        }

        internal override async void Encode()
        {
            ClientHome ownerHome = new ClientHome(Player.Avatar.GetId());
            ownerHome.SetShieldTime(Player.Avatar.GetShieldTime);
            ownerHome.SetProtectionTime(Player.Avatar.GetProtectionTime);
            ownerHome.SetHomeJSON(JsonBase);

            this.Data.AddInt(0);
            this.Data.AddInt((int) Player.Avatar.LastTickSaved.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            this.Data.AddRange(ownerHome.Encode());
            this.Data.AddRange(await Player.Avatar.Encode());
            this.Data.AddInt(LevelId);

        }

        public string JsonBase;
        public int LevelId;
        public Level Player;
    }
}