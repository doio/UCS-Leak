using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeCancelMessage : Message
    {
        public ChallangeCancelMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal async void Process()
        {
            try
            {
                Alliance a = ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceId);
                StreamEntry s = a.m_vChatMessages.Find(c => c.m_vSenderId == this.Device.Player.Avatar.AllianceId && c.GetStreamEntryType() == 12);

                if (s != null)
                {
                    a.m_vChatMessages.RemoveAll(t => t == s);
                    foreach (AllianceMemberEntry op in a.GetAllianceMembers())
                    {
                        Level player = await ResourcesManager.GetPlayer(op.AvatarId);
                        if (player.Client != null)
                        {
                            new AllianceStreamEntryRemovedMessage(Device, s.m_vId).Send();
                        }
                    }
                }
                else
                {
                    new OutOfSyncMessage(this.Device).Send();
                }
            } catch (Exception) { }
        }

    }
}
