using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Commands.Client;
using UCS.Packets.Commands.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    // Packet 14310
    internal class DonateAllianceUnitMessage : Message
    {
        public CombatItemData Troop;
        public int MessageID;
        public byte BuyTroop;

        public DonateAllianceUnitMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            this.Troop = (CombatItemData) this.Reader.ReadDataReference();
            this.Reader.ReadInt32();
            this.MessageID = this.Reader.ReadInt32();
        }

        internal override async void Process()
        {
            try
            {
                Alliance a = await ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceID);
                StreamEntry stream = a.GetChatMessages().Find(c => c.GetId() == MessageID);
                Level _Sender = await ResourcesManager.GetPlayer(stream.GetSenderId());
                int upcomingspace = stream.m_vDonatedTroop + Troop.GetHousingSpace();

                if (upcomingspace <= stream.m_vMaxTroop)
                {

                    /*DonatedAllianceUnitCommand _Donated = new DonatedAllianceUnitCommand(this.Device);
                    _Donated.Tick(_Sender);
                    _Donated.SetDonator(this.Device.Player.Avatar.Username);
                    _Donated.SetUnitID(Troop.GetGlobalID());
                    _Donated.SetUnitLevel(this.Device.Player.Avatar.GetUnitUpgradeLevel(Troop));*/

                    //new AvailableServerCommandMessage(this.Device, _Donated.Handle()).Send();

                    StreamEntry _Stream = a.GetChatMessages().Find(c => c.GetId() == MessageID);
                    Level _PreviousPlayer = await ResourcesManager.GetPlayer(_Stream.GetSenderId());
                    ClientAvatar _PreviousPlayerAvatar = _PreviousPlayer.Avatar;
                    _Stream.AddDonatedTroop(this.Device.Player.Avatar.UserID, Troop.GetGlobalID(), 1,
                        this.Device.Player.Avatar.GetUnitUpgradeLevel(Troop));
                    int _Capicity = Troop.GetHousingSpace();
                    _Stream.AddUsedCapicity(_Capicity);
                    _PreviousPlayerAvatar.Castle_Used += _Capicity;
                    _PreviousPlayerAvatar.Add_Castle_Unit(this.Device.Player.Avatar.UserID, Troop.GetGlobalID(), 1, this.Device.Player.Avatar.GetUnitUpgradeLevel(Troop));

                    foreach (AllianceMemberEntry op in a.GetAllianceMembers())
                    {
                        Level player = await ResourcesManager.GetPlayer(op.AvatarID);
                        if (player.Client != null)
                        {
                            AllianceStreamEntryMessage c = new AllianceStreamEntryMessage(player.Client);
                            c.SetStreamEntry(_Stream);
                            c.Send();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
