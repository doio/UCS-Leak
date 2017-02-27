using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.Packets.Messages.Server;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Messages.Client
{
    internal class ChallangeAttackMessage : Message
    {
        public ChallangeAttackMessage(Packets.Client client, PacketReader br) : base(client, br)
        {
        }

        public long ID { get; set; }

        public override void Decode()
        {
            using (PacketReader br = new PacketReader(new MemoryStream(GetData())))
            {
                ID = br.ReadInt64WithEndian();
            }
        }

        public override async void Process(Level level)
        {
            try
            {
                if (level.GetPlayerAvatar().State == ClientAvatar.UserState.CHA)
                {
                    ResourcesManager.DisconnectClient(Client);
                }
                else
                {
                    level.GetPlayerAvatar().State = ClientAvatar.UserState.CHA;
                    Alliance a = await ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
                    Level defender = await ResourcesManager.GetPlayer(a.GetChatMessages().Find(c => c.GetId() == ID).GetSenderId());
                    if (defender != null)
                    {
                        defender.Tick();
                        PacketProcessor.Send(new ChallangeAttackDataMessage(Client, defender));
                    }
                    else
                    {
                        new OwnHomeDataMessage(Client, level);
                    }

                    Alliance alliance = await ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
                    StreamEntry s = alliance.GetChatMessages().Find(c => c.GetStreamEntryType() == 12);
                    if (s != null)
                    {
                        alliance.GetChatMessages().RemoveAll(t => t == s);

                        foreach (AllianceMemberEntry op in alliance.GetAllianceMembers())
                        {
                            Level playera = await ResourcesManager.GetPlayer(op.GetAvatarId());
                            if (playera.GetClient() != null)
                            {
                                AllianceStreamEntryMessage p = new AllianceStreamEntryMessage(playera.GetClient());
                                p.SetStreamEntry(s);
                                PacketProcessor.Send(p);
                            }
                        }
                    }
                }
            } catch (Exception) { }
        }
    }
}
