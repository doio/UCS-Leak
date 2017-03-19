using UCS.Helpers.List;
using UCS.Logic;

namespace  UCS.Packets.Commands.Server
{
    //Command 6
    internal class AllianceSettingChangedCommand : Command
    {
        private Alliance m_vAlliance;
        private Level m_vPlayer;

        public AllianceSettingChangedCommand(Device client) : base(client)
        {
            this.Identifier = 6;
        }

        internal override void Encode()
        {
            m_vPlayer.Tick();
            this.Data.AddLong(m_vAlliance.AllianceID);
            this.Data.AddInt(m_vAlliance.GetAllianceBadgeData());
            this.Data.AddInt(m_vAlliance.GetAllianceLevel());
            this.Data.AddInt(0); //Tick
        }

        public void SetAlliance(Alliance alliance)
        {
            m_vAlliance = alliance;
        }

        public void SetPlayer(Level player)
        {
            m_vPlayer = player;
        }
    }
}