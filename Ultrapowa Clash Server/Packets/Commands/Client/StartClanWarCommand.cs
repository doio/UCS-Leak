using System;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers.Binary;
using UCS.Logic;
using UCS.Packets.Messages.Server;

namespace UCS.Packets.Commands.Client
{
    internal class StartClanWarCommand : Command
    {
        public StartClanWarCommand(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.Tick = this.Reader.ReadInt32();
        }

        public int Tick;

        internal override async void Process()
        {
            try
            {
                Alliance an = await ObjectManager.GetAlliance(this.Device.Player.Avatar.AllianceID);
                if (an != null)
                {
                    if (an.GetAllianceMembers().Count >= 10)
                    {
                        foreach (AllianceMemberEntry a in an.GetAllianceMembers())
                        {
                            Level l = await ResourcesManager.GetPlayer(a.GetAvatarId());
                            new AllianceWarMapDataMessage(this.Device).Send();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
