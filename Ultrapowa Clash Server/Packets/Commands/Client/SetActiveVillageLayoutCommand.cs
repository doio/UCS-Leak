using UCS.Helpers.Binary;

namespace UCS.Packets.Commands.Client
{
    // Packet 567
    internal class SetActiveVillageLayoutCommand : Command
    {
        public SetActiveVillageLayoutCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        private int Layout;

        internal override void Decode()
        {
            this.Layout = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
           /* this.Device.Player.Avatar.SetActiveLayout(this.Layout);

            System.Console.WriteLine(this.Layout);
            System.Console.WriteLine(this.Device.Player.Avatar.GetActiveLayout());*/
        }
    }
}
