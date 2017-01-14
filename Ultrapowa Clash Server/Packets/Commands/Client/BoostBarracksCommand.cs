using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    internal class BoostBarracksCommand : Command
    {
        public BoostBarracksCommand(PacketReader br) 
        {
            Tick = br.ReadInt64();
        }

        public long Tick { get; set; }

        public override void Execute(Level level)
        {
           /* var player = level.GetPlayerAvatar();
            var barracks = level.GameObjectManager.GetGameObjectByID(500000010);
            var boost = (Building)barracks;

            if(!boost.IsBoosted)
            {
                boost.BoostBuilding();
            } */
        }
    }
}
