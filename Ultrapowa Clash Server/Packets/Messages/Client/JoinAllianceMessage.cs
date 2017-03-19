using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Commands.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14305
    internal class JoinAllianceMessage : Message
    {
        long m_vAllianceId;

        public JoinAllianceMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
            this.m_vAllianceId = this.Reader.ReadInt64();
        }

        internal override async void Process()
        {
            try
            {
                Alliance alliance = await ObjectManager.GetAlliance(m_vAllianceId);
                if (alliance != null)
                {
                    if (!alliance.IsAllianceFull())
                    {
                        this.Device.Player.Avatar.AllianceID = alliance.AllianceID;
                        AllianceMemberEntry member = new AllianceMemberEntry(this.Device.Player.Avatar.UserID);
                        member.SetRole(1);
                        alliance.AddAllianceMember(member);

                        JoinedAllianceCommand b = new JoinedAllianceCommand(this.Device);
                        b.SetAlliance(alliance);

                        AllianceRoleUpdateCommand c = new AllianceRoleUpdateCommand(this.Device);
                        c.SetAlliance(alliance);
                        c.SetRole(1);
                        c.Tick(this.Device.Player);

                        new AvailableServerCommandMessage(this.Device, b.Handle()).Send();

                        new AvailableServerCommandMessage(this.Device, c.Handle()).Send();

                        new AllianceStreamMessage(Device, alliance).Send();
                    }
                }
            } catch (Exception) { }
        }
    }
}
