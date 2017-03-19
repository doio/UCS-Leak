using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;
using UCS.Helpers.Binary;
namespace UCS.Packets.Messages.Client
{
    // Packet 14113
    internal class VisitHomeMessage : Message
    {
        public VisitHomeMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal long AvatarId;

        internal override void Decode()
        {
            this.AvatarId = this.Reader.ReadInt64();
        }

        internal override async void Process()
        {
            try
            {
                Level targetLevel = await ResourcesManager.GetPlayer(AvatarId);
                targetLevel.Tick();
                new VisitedHomeDataMessage(Device, targetLevel, this.Device.Player).Send();


                if (this.Device.Player.Avatar.AllianceID > 0)
                {
                    Alliance alliance = await ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceID);
                    if (alliance != null)
                    {
                        new AllianceStreamMessage(Device, alliance).Send();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
