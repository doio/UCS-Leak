using System;
using UCS.Helpers.List;
using UCS.Logic;

namespace UCS.Packets.Messages.Server
{
    // Packet 24113
    internal class VisitedHomeDataMessage : Message
    {
        public VisitedHomeDataMessage(Device client, Level ownerLevel, Level visitorLevel) : base(client)
        {
            this.Identifier = 24113;
            m_vOwnerLevel = ownerLevel;
            m_vVisitorLevel = visitorLevel;
        }

        internal override async void Encode()
        {
            try
            {
                this.Device.PlayerState = Logic.Enums.State.VISIT;
                ClientHome ownerHome = new ClientHome(m_vOwnerLevel.Avatar.UserID);
                ownerHome.SetShieldTime(m_vOwnerLevel.Avatar.Shield);
                ownerHome.SetProtectionTime(m_vOwnerLevel.Avatar.Guard);
                ownerHome.SetHomeJSON(m_vOwnerLevel.SaveToJSON());

                this.Data.AddInt(-1);
                this.Data.AddInt((int)TimeSpan.FromSeconds(100).TotalSeconds);
                this.Data.AddRange(ownerHome.Encode());
                this.Data.AddRange(await this.m_vOwnerLevel.Avatar.Encode());
                this.Data.AddInt(0);
                this.Data.Add(1);
                this.Data.AddRange(await this.m_vVisitorLevel.Avatar.Encode());
            }
            catch (Exception)
            {
            }
        }

        readonly Level m_vOwnerLevel;
        readonly Level m_vVisitorLevel;
    }
}
