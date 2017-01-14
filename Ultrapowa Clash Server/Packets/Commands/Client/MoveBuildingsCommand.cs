using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.Manager;

namespace UCS.Packets.Commands.Client
{
    class MoveBuildingsCommand : Command
    {
        public MoveBuildingsCommand(PacketReader br)
        {
            MovedBuilding = br.ReadInt32WithEndian();
            ReplacedBuilding = br.ReadInt32WithEndian();
            Tick = br.ReadInt32WithEndian();
        }

        public int Tick { get; set; }

        public int ReplacedBuilding { get; set; }

        public int MovedBuilding { get; set; }

        public override void Execute(Level level)
        {
            if(MovedBuilding != null)
            {
                Vector movedBuildingPosition = level.GameObjectManager.GetGameObjectByID(MovedBuilding).GetPosition();
                Vector replacedBuildingPosition = level.GameObjectManager.GetGameObjectByID(ReplacedBuilding).GetPosition();

                level.GameObjectManager.GetGameObjectByID(MovedBuilding).SetPositionXY(Convert.ToInt32(replacedBuildingPosition.X), Convert.ToInt32(replacedBuildingPosition.Y), level.GetPlayerAvatar().GetActiveLayout());
                level.GameObjectManager.GetGameObjectByID(ReplacedBuilding).SetPositionXY(Convert.ToInt32(movedBuildingPosition.X), Convert.ToInt32(movedBuildingPosition.Y), level.GetPlayerAvatar().GetActiveLayout());
            }
        }
    }
}
