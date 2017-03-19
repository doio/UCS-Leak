using System;
using System.Collections.Generic;
using UCS.Core;
using UCS.Files.Logic;

namespace UCS.Logic
{
    internal class Avatar
    {
        public Avatar()
        {
            m_vResources = new List<DataSlot>();
            m_vResourceCaps = new List<DataSlot>();
            m_vUnitCount = new List<DataSlot>();
            m_vUnitUpgradeLevel = new List<DataSlot>();
            m_vHeroHealth = new List<DataSlot>();
            m_vHeroUpgradeLevel = new List<DataSlot>();
            m_vHeroState = new List<DataSlot>();
            m_vSpellCount = new List<DataSlot>();
            m_vSpellUpgradeLevel = new List<DataSlot>();
        }

        protected List<DataSlot> m_vHeroHealth;
        protected List<DataSlot> m_vHeroState;
        protected List<DataSlot> m_vHeroUpgradeLevel;
        protected List<DataSlot> m_vResourceCaps;
        protected List<DataSlot> m_vResources;
        protected List<DataSlot> m_vSpellCount;
        protected List<DataSlot> m_vSpellUpgradeLevel;
        protected List<DataSlot> m_vUnitCount;
        protected List<DataSlot> m_vUnitUpgradeLevel;

        int m_vCastleLevel = -1;
        int m_vCastleTotalCapacity;
        int m_vCastleUsedCapacity;
        int m_vTownHallLevel;

        public static int GetDataIndex(List<DataSlot> dsl, Data d) => dsl.FindIndex(ds => ds.Data == d);

        public int GetAllianceCastleLevel() => m_vCastleLevel;

        public int GetAllianceCastleTotalCapacity() => m_vCastleTotalCapacity;

        public int GetAllianceCastleUsedCapacity() => m_vCastleUsedCapacity;
    }
}
