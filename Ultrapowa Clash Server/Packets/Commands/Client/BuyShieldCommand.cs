using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Files.Logic;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Logic.Enums;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    // Packet 522
    internal class BuyShieldCommand : Command
    {
        public BuyShieldCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.ShieldId = this.Reader.ReadInt32();
            this.Tick = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            ShieldData Shield = CSVManager.DataTables.GetDataById(this.ShieldId) as ShieldData;


            if (Shield != null)
            {
                if (this.Device.Player.Avatar.Resources.Get(Resource.Diamonds) >= Shield.Diamonds)
                {
                    this.Device.Player.Avatar.Shield += Shield.TimeH * 3600;
                    this.Device.Player.Avatar.Guard += Shield.GuardTimeH * 3600;

                    this.Device.Player.Avatar.Resources.Minus(Resource.Diamonds, Shield.Diamonds);
                }
                else
                    new OutOfSyncMessage(this.Device).Send();
            }
            else
                new OutOfSyncMessage(this.Device).Send();
        }

        public int ShieldId;
        public int Tick;
    }
}
