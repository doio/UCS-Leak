using System;
using System.IO;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;

namespace UCS.Packets.Messages.Client
{
    // Packet 14310
    internal class DonateAllianceUnitMessage : Message
    {
        public int Unknown1;
        public CombatItemData Troop;
        public int Unknown3;
        public int MessageID;

        public DonateAllianceUnitMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                Unknown1 = br.ReadInt32WithEndian();
                Troop = (CombatItemData)br.ReadDataReference();
                Unknown3 = br.ReadInt32WithEndian();
                MessageID = br.ReadInt32WithEndian();
            }
        }
        public override async void Process(Level level)
        {
            try
            {
                Alliance a = await ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
                StreamEntry stream = a.GetChatMessages().Find(c => c.GetId() == MessageID);

                Level sender = await ResourcesManager.GetPlayer(stream.GetSenderId());
                int upcomingspace = stream.m_vDonatedTroop + Troop.GetHousingSpace();
                if (upcomingspace <= stream.m_vMaxTroop)
                {
                    //System.Console.WriteLine("Troop Donated :" + Troop.GetName());
                }
            } catch (Exception) { }
        }

    }
}
