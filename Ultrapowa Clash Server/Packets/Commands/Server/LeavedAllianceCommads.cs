using System;
using System.Collections.Generic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.Packets.Commands.Server
{
    //Command 2
    internal class LeavedAllianceCommand : Command
    {
        private Alliance m_vAlliance;
        private int m_vReason;

        public LeavedAllianceCommand()
        {
        }

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt64(m_vAlliance.GetAllianceId());
            data.AddInt32(m_vReason);
            data.AddInt32(-1); //Tick Probably
            return data.ToArray();
        }

        public void SetAlliance(Alliance alliance)
        {
            m_vAlliance = alliance;
        }

        public void SetReason(int reason)
        {
            m_vReason = reason;
        }

        //00 00 07 3A
        //00 00 00 01 ////reason? 1= leave, 2=kick

        //00 00 00 3B 00 0A 40 1E
    }
}
