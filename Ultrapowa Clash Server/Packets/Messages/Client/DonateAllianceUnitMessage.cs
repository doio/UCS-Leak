using System;
using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers;
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

        public DonateAllianceUnitMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                br.ReadInt32WithEndian();
                Troop = (CombatItemData)br.ReadDataReference();
                br.ReadInt32WithEndian();
                MessageID = br.ReadInt32WithEndian();
            }
        }
        public override async void Process(Level level)
        {
            try
            {
                Alliance a = await ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
                StreamEntry stream = a.GetChatMessages().Find(c => c.GetId() == MessageID);
                Level _Sender = await ResourcesManager.GetPlayer(stream.GetSenderId());
                int upcomingspace = stream.m_vDonatedTroop + Troop.GetHousingSpace();

                if (upcomingspace <= stream.m_vMaxTroop)
                {
                    DonateAllianceUnitCommand _Donate = new DonateAllianceUnitCommand();
                    _Donate.SetMessageID(MessageID);
                    _Donate.Tick(level);
                    _Donate.SetUnit(Troop);

                    DonatedAllianceUnitCommand _Donated = new DonatedAllianceUnitCommand();
                    _Donated.Tick(_Sender);
                    _Donated.SetDonator(level.GetPlayerAvatar().GetAvatarName());
                    _Donated.SetUnitID(Troop.GetGlobalID());
                    _Donated.SetUnitLevel(level.GetPlayerAvatar().GetUnitUpgradeLevel(Troop));

                    AvailableServerCommandMessage availableServerCommandMessage = new AvailableServerCommandMessage(level.GetClient());
                    availableServerCommandMessage.SetCommandId(4);
                    availableServerCommandMessage.SetCommand(_Donate);

                    AvailableServerCommandMessage ravailableServerCommandMessage = new AvailableServerCommandMessage(_Sender.GetClient());
                    ravailableServerCommandMessage.SetCommandId(5);
                    ravailableServerCommandMessage.SetCommand(_Donated);

                    StreamEntry _Stream = a.GetChatMessages().Find(c => c.GetId() == MessageID);
                    Level _PreviousPlayer = await ResourcesManager.GetPlayer(_Stream.GetSenderId());
                    ClientAvatar _PreviousPlayerAvatar = _PreviousPlayer.GetPlayerAvatar();
                    _Stream.AddDonatedTroop(level.GetPlayerAvatar().GetId(), Troop.GetGlobalID(), 1, level.GetPlayerAvatar().GetUnitUpgradeLevel(Troop));
                    int _Capicity = Troop.GetHousingSpace();
                    _Stream.AddUsedCapicity(_Capicity);
                    _PreviousPlayerAvatar.SetAllianceCastleUsedCapacity(_PreviousPlayerAvatar.GetAllianceCastleUsedCapacity() + _Capicity);
                    _PreviousPlayerAvatar.AddAllianceTroop(level.GetPlayerAvatar().GetId(), Troop.GetGlobalID(), 1, level.GetPlayerAvatar().GetUnitUpgradeLevel(Troop));

                    //PacketProcessor.Send(availableServerCommandMessage);
                    if (ResourcesManager.IsPlayerOnline(_Sender))
                    {
                        PacketProcessor.Send(ravailableServerCommandMessage);
                    }

                    foreach (AllianceMemberEntry op in a.GetAllianceMembers())
                    {
                        Level player = await ResourcesManager.GetPlayer(op.GetAvatarId());
                        if (player.GetClient() != null)
                        {
                            AllianceStreamEntryMessage c = new AllianceStreamEntryMessage(player.GetClient());
                            c.SetStreamEntry(_Stream);
                            PacketProcessor.Send(c);
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
