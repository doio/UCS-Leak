using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeAttackMessage : Message
    {
        public ChallangeAttackMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public long ID { get; set; }

        internal override void Decode()
        {
            this.ID = this.Reader.ReadInt64();
        }

        internal async void Process()
        {
            try
            {
                if (this.Device.PlayerState == Logic.Enums.State.IN_BATTLE)
                {
                    ResourcesManager.DisconnectClient(Device);
                }
                else
                {
                    this.Device.PlayerState = Logic.Enums.State.IN_BATTLE;
                    Alliance a = ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceId);
                    Level defender = await ResourcesManager.GetPlayer(a.m_vChatMessages.Find(c => c.GetId() == ID).GetSenderId());
                    if (defender != null)
                    {
                        defender.Tick();
                        new ChallangeAttackDataMessage(Device, defender).Send();
                    }
                    else
                    {
                        new OwnHomeDataMessage(Device, this.Device.Player).Send();
                    }

                    Alliance alliance = ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceId);
                    StreamEntry s = alliance.m_vChatMessages.Find(c => c.GetStreamEntryType() == 12);
                    if (s != null)
                    {
                        alliance.m_vChatMessages.RemoveAll(t => t == s);

                        foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                        {
                            Level playera = await ResourcesManager.GetPlayer(op.AvatarId);
                            if (playera.Client != null)
                            {
                                AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(playera.Client);
                                p.SetStreamEntry(s);
                                p.Send();
                            }
                        }
                    }
                }
            } catch (Exception) { }
        }
    }
}
