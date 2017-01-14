using System.IO;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 514
    internal class SpeedUpClearingCommand : Command
    {
        readonly int m_vObstacleId;

        readonly int m_vTick;

        public SpeedUpClearingCommand(PacketReader br)
        {
            m_vObstacleId = br.ReadInt32WithEndian();
            m_vTick = br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            /*GameObject gameObjectByID = level.GameObjectManager.GetGameObjectByID(m_vObstacleId);
            if (gameObjectByID != null && gameObjectByID.ClassId == 3)
            {
                ((Obstacle)gameObjectByID).SpeedUpClearing();
            }
            */
        }
    }
}
