using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 546
    internal class EditVillageLayoutCommand : Command
    {
        private int X;
        private int Y;
        private int BuildingID;
        private int Layout;

        public EditVillageLayoutCommand(PacketReader br)
        {
            X = br.ReadInt32();
            Y = br.ReadInt32();
            BuildingID = br.ReadInt32();
            Layout = br.ReadInt32();
            br.ReadInt32();
        }

        public override void Execute(Level level)
        {
            /*if (Layout != level.GetPlayerAvatar().GetActiveLayout())
            {
                GameObject go = level.GameObjectManager.GetGameObjectByID(BuildingID);
                go.SetPositionXY(X, Y, Layout);
            } */
        }

    }
}
