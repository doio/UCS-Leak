using UCS.Helpers.Binary;

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

        internal override void Process()
        {
            /*Alliance an = ObjectManager.GetAlliance(level.Avatar.GetAllianceId());
            if (an != null)
            {
                if(an.GetAllianceMembers().Count >= 10)
                {
                    foreach(AllianceMemberEntry a in an.GetAllianceMembers())
                    {
                        Level l = ResourcesManager.GetPlayer(a.GetAvatarId());
                        new AllianceWarMapDataMessage(l.Client).Send();
                    }
                }
            }*/
        }
    }
}
