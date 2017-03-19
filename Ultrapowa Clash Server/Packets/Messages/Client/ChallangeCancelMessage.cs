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
                Alliance a = await ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceID);
                StreamEntry s = a.GetChatMessages().Find(c => c.GetSenderId() == this.Device.Player.Avatar.UserID && c.GetStreamEntryType() == 12);

                if (s != null)
                {
                    a.GetChatMessages().RemoveAll(t => t == s);
                    foreach (AllianceMemberEntry op in a.GetAllianceMembers())
                    {
                        Level player = await ResourcesManager.GetPlayer(op.GetAvatarId());
                        if (player.Client != null)
                        {
                            new AllianceStreamEntryRemovedMessage(player.Client, s.GetId()).Send();
                        }
                    }
                }
            } catch (Exception) { }
        }

    }
}
