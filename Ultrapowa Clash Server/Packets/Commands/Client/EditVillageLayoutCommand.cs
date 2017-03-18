using UCS.Helpers.Binary;
using UCS.Logic;

namespace UCS.Packets.Commands.Client
{
    // Packet 546
    internal class EditVillageLayoutCommand : Command
    {
        internal int X;
        internal int Y;
        internal int BuildingID;
        internal int Layout;

        public EditVillageLayoutCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.BuildingID = this.Reader.ReadInt32();
            this.Layout = this.Reader.ReadInt32();
            this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            /*if (this.Layout != this.Device.Player.Avatar.GetActiveLayout())
            {
                GameObject go = this.Device.Player.GameObjectManager.GetGameObjectByID(BuildingID);
                go.SetPositionXY(X, Y, this.Layout);
            }*/

            //System.Console.WriteLine(this.Layout);
        }

    }
}
