using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 506
    internal class CollectResourcesCommand : Command
    {
        public CollectResourcesCommand(PacketReader br)
        {
            BuildingId = br.ReadInt32WithEndian(); 
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);

            if (go != null)
            {
                if (go.ClassId == 0 || go.ClassId == 4)
                {
                    var constructionItem = (ConstructionItem) go;
                    constructionItem.GetResourceProductionComponent().CollectResources();
                }
            }
        }

        public int BuildingId { get; set; }
        public uint Unknown1 { get; set; }
    }
}