using System;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    internal class ChallangeAttackDataMessage : Message
    {
        internal readonly Level OwnerLevel;
        internal readonly Level VisitorLevel;

        public ChallangeAttackDataMessage(Device client, Level level) : base(client)
        {
            this.Identifier = 24107;
            this.OwnerLevel = level;
            this.VisitorLevel = client.Player;
        }

        internal override async void Encode()
        {
            try
            {
                ClientHome ch = new ClientHome(this.OwnerLevel.Avatar.UserID)
                {
                    Village = this.OwnerLevel.SaveToJSON(),
                    Amical = false
                };
                this.Data.AddInt((int)TimeSpan.FromSeconds(100).TotalSeconds);
                this.Data.AddInt(-1);
                this.Data.AddInt((int)this.OwnerLevel.Avatar.Update.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                this.Data.AddRange(ch.Encode());
                this.Data.AddRange(await this.OwnerLevel.Avatar.Encode());
                this.Data.AddRange(await this.VisitorLevel.Avatar.Encode());
                this.Data.AddInt(1); // 1 : Amical ?       2 : next button disabled       3 : PvP         5 : Amical again ?
                this.Data.AddInt(0);
                this.Data.Add(0);
            }
            catch (Exception)
            {
            }
        }
    }
}

