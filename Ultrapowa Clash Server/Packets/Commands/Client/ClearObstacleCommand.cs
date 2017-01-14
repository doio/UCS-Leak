using System;
using System.IO;
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 507
    internal class ClearObstacleCommand : Command
    {
        public ClearObstacleCommand(PacketReader br)
        {
            ObstacleId = br.ReadInt32WithEndian();
            Tick = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            /*ClientAvatar playerAvatar = level.GetPlayerAvatar();
            Obstacle gameObjectByID = (Obstacle)level.GameObjectManager.GetGameObjectByID(ObstacleId);
            ObstacleData obstacleData = gameObjectByID.GetObstacleData();
            if (playerAvatar.HasEnoughResources(obstacleData.GetClearingResource(), obstacleData.ClearCost) && level.HasFreeWorkers())
            {
                ResourceData clearingResource = obstacleData.GetClearingResource();
                playerAvatar.SetResourceCount(clearingResource, playerAvatar.GetResourceCount(clearingResource) - obstacleData.ClearCost);
                gameObjectByID.StartClearing();
            }*/
        }

        public int ObstacleId { get; set; }
        public uint Tick { get; set; }
    }
}
