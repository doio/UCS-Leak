using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    class PlaceAllianceTroopsCommand : Command
    {
        public PlaceAllianceTroopsCommand(PacketReader br)
        {
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
            br.ReadUInt32WithEndian();
        }

        public override async void Execute(Level level)
        {
            ClientAvatar _Player = level.GetPlayerAvatar();

            if (_Player != null)
            {
                _Player.AllianceUnits.Clear();
                _Player.SetAllianceCastleUsedCapacity(0);
            }
        }
    }
}
