using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    internal class StartClanWarCommand : Command
    {
        public StartClanWarCommand(PacketReader br)
        {
            Tick = br.ReadInt32WithEndian();
        }

        public int Tick { get; set; }

        public override void Execute(Level level)
        {
            /*Alliance an = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            if (an != null)
            {
                if(an.GetAllianceMembers().Count >= 10)
                {
                    foreach(AllianceMemberEntry a in an.GetAllianceMembers())
                    {
                        Level l = ResourcesManager.GetPlayer(a.GetAvatarId());
                        new AllianceWarMapDataMessage(l.GetClient()).Send();
                    }
                }
            }*/
        }
    }
}
