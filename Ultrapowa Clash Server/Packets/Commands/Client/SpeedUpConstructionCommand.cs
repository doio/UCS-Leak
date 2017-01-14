using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 504
    internal class SpeedUpConstructionCommand : Command
    {
        readonly int m_vBuildingId;

        public SpeedUpConstructionCommand(PacketReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
            {
                if (go.ClassId == 0 || go.ClassId == 4)
                {
                    ((ConstructionItem) go).SpeedUpConstruction();
                }
            }
        }
    }
}