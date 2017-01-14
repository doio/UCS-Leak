using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 525
    internal class LoadTurretCommand : Command
    {
        public LoadTurretCommand(PacketReader br)
        {
            m_vUnknown1 = br.ReadUInt32WithEndian();
            m_vBuildingId = br.ReadInt32WithEndian();
            m_vUnknown2 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
                if (go.GetComponent(1, true) != null)
                    ((CombatComponent) go.GetComponent(1, true)).FillAmmo();
        }

        public int m_vBuildingId { get; set; }
        public uint m_vUnknown1 { get; set; }
        public uint m_vUnknown2 { get; set; }
    }
}