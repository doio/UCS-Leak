using System;
using System.Collections.Generic;
using UCS.Core;
using UCS.Files.Logic;

namespace UCS.Logic
{
    internal class Avatar
    {
        protected List<DataSlot> m_heroHealth;
        protected List<DataSlot> m_heroState;
        protected List<DataSlot> m_heroUpgradeLevel;
        protected List<DataSlot> m_hesourceCaps;
        protected List<DataSlot> m_hesources;
        protected List<DataSlot> m_spellCount;
        protected List<DataSlot> m_spellUpgradeLevel;
        protected List<DataSlot> m_unitCount;
        protected List<DataSlot> m_unitUpgradeLevel;

        int m_castleLevel = -1;
        int m_castleTotalCapacity;
        int m_castleUsedCapacity;
        internal int m_townHallLevel;

        public Avatar()
        {
            m_resources         = new List<DataSlot>();
            m_resourceCaps      = new List<DataSlot>();
            m_unitCount         = new List<DataSlot>();
            m_unitUpgradeLevel  = new List<DataSlot>();
            m_heroHealth        = new List<DataSlot>();
            m_heroUpgradeLevel  = new List<DataSlot>();
            m_heroState         = new List<DataSlot>();
            m_spellCount        = new List<DataSlot>();
            m_spellUpgradeLevel = new List<DataSlot>();
        }

        public static int GetDataIndex(List<DataSlot> dsl, Data d)
        dsl.FindIndex(ds => ds.Data == d);

        public void CommodityCountChangeHelper(int commodityType, Data data, int count)
        {
            if (data.GetDataType() == 2)
            {
                if (commodityType == 0)
                {
                    int resourceCount = GetResourceCount((ResourceData) data);
                    int newResourceValue = Math.Max(resourceCount + count, 0);
                    if (count >= 1)
                    {
                        int resourceCap = GetResourceCap((ResourceData) data);
                        if (resourceCount < resourceCap)
                        {
                            if (newResourceValue > resourceCap)
                            {
                                newResourceValue = resourceCap;
                            }
                        }
                    }
                    SetResourceCount((ResourceData) data, newResourceValue);
                }
            }
        }

        public int GetAllianceCastleLevel() => m_vCastleLevel;

        public int GetAllianceCastleTotalCapacity() => m_vCastleTotalCapacity;

        public int GetAllianceCastleUsedCapacity() => m_vCastleUsedCapacity;

        public int GetResourceCap(ResourceData rd)
        {
            int index = GetDataIndex(m_vResourceCaps, rd);
            int count = 0;
            if (index != -1)
            {
                count = m_vResourceCaps[index].Value;
            }
            return count;
        }

        public List<DataSlot> GetResourceCaps() => m_vResourceCaps;

        public int GetResourceCount(ResourceData rd)
        {
            int index = GetDataIndex(m_vResources, rd);
            int count = 0;
            if (index != -1)
            {
                count = m_vResources[index].Value;
            }
            return count;
        }

        public List<DataSlot> GetResources() => m_vResources;

        public List<DataSlot> GetSpells() => m_vSpellCount;

        public int GetUnitCount(CombatItemData cd)
        {
            int result = 0;
            if (cd.GetCombatItemType() == 1)
            {
                int index = GetDataIndex(m_vSpellCount, cd);
                if (index != -1)
                {
                    result = m_vSpellCount[index].Value;
                }
            }
            else
            {
                int index = GetDataIndex(m_vUnitCount, cd);
                if (index != -1)
                {
                    result = m_vUnitCount[index].Value;
                }
            }
            return result;
        }

        public List<DataSlot> GetUnits() => m_vUnitCount;

        public int GetUnitUpgradeLevel(CombatItemData cd)
        {
            int result = 0;
            switch (cd.GetCombatItemType())
            {
                case 2:
                    {
                        int index = GetDataIndex(m_vHeroUpgradeLevel, cd);
                        if (index != -1)
                        {
                            result = m_vHeroUpgradeLevel[index].Value;
                        }
                        break;
                    }
                case 1:
                    {
                        int index = GetDataIndex(m_vSpellUpgradeLevel, cd);
                        if (index != -1)
                        {
                            result = m_vSpellUpgradeLevel[index].Value;
                        }
                        break;
                    }

                default:
                    {
                        int index = GetDataIndex(m_vUnitUpgradeLevel, cd);
                        if (index != -1)
                        {
                            result = m_vUnitUpgradeLevel[index].Value;
                        }
                        break;
                    }
            }
            return result;
        }

        public int GetUnusedResourceCap(ResourceData rd)
        {
            int resourceCount = GetResourceCount(rd);
            int resourceCap   = GetResourceCap(rd);
            return Math.Max(resourceCap - resourceCount, 0);
        }

        public void SetAllianceCastleLevel(int level)
        {
            m_vCastleLevel = level;
        }

        public void IncrementAllianceCastleLevel()
        {
            m_vCastleLevel++;
        }
        public void DeIncrementAllianceCastleLevel()
        {
            m_vCastleLevel--;
        }

        public void SetTownHallLevel(int level)
        {
            m_vTownHallLevel = level;
        }

        public void IncrementTownHallLevel()
        {
            m_vTownHallLevel++;
        }

        public void DeIncrementTownHallLevel()
        {
            m_vTownHallLevel--;
        }

        public void SetAllianceCastleTotalCapacity(int totalCapacity)
        {
            m_vCastleTotalCapacity = totalCapacity;
        }

        public void SetAllianceCastleUsedCapacity(int usedCapacity)
        {
            m_vCastleUsedCapacity = usedCapacity;
        }

        public void SetHeroHealth(HeroData hd, int health)
        {
            int index = GetDataIndex(m_vHeroHealth, hd);
            if (index == -1)
            {
                DataSlot ds = new DataSlot(hd, health);
                m_vHeroHealth.Add(ds);
            }

            else
            {
                m_heroHealth[index].Value = health;
            }
        }

        public void SetHeroState(HeroData hd, int state)
        {
            int index = GetDataIndex(this.m_heroState, hd);

            if (index == -1)
            {
                DataSlot ds = new DataSlot(hd, state);
                m_heroState.Add(ds);
            }

            else
            {
                m_heroState[index].Value = state;
            }
        }

        public void SetResourceCap(ResourceData rd, int value)
        {
            int index = GetDataIndex(this.m_resourceCaps, rd);

            if (index == -1)
            {
                DataSlot ds = new DataSlot(rd, value);
                m_resourceCaps.Add(ds);
            }

            else
            {
                m_resourceCaps[index].Value = value;
            }
        }

        public void SetResourceCount(ResourceData rd, int value)
        {
            int index = GetDataIndex(this.m_resources, rd);

            if (index == -1)
            {
                DataSlot ds = new DataSlot(rd, value);
                m_resources.Add(ds);
            }

            else
            {
                m_resources[index].Value = value;
            }
        }

        public void SetUnitCount(CombatItemData cd, int count)
        {
            switch (cd.GetCombatItemType())
            {
                    case 1:
                    {
                        int index = GetDataIndex(this.m_spellCount, cd);

                        if (index != -1)
                        {
                            m_spellCount[index].Value = count;
                        }

                        else
                        {
                            DataSlot ds = new DataSlot(cd, count);
                            m_spellCount.Add(ds);
                        }

                        break;
                    }

                    default:
                    {
                        int index = GetDataIndex(this.m_unitCount, cd);

                        if (index != -1)
                        {
                            m_unitCount[index].Value = count;
                        }

                        else
                        {
                            DataSlot ds = new DataSlot(cd, count);
                            m_unitCount.Add(ds);
                        }

                        break;
                    }
            }
        }

        public void SetUnitUpgradeLevel(CombatItemData cd, int level)
        {
            switch (cd.GetCombatItemType())
            {
                case 2:
                    {
                        int index = GetDataIndex(m_vHeroUpgradeLevel, cd);

                        if (index != -1)
                        {
                            m_heroUpgradeLevel[index].Value = level;
                        }

                        else
                        {
                            DataSlot ds = new DataSlot(cd, level);
                            m_heroUpgradeLevel.Add(ds);
                        }

                        break;
                    }

                    case 1:
                    {
                        int index = GetDataIndex(this.m_spellUpgradeLevel, cd);

                        if (index != -1)
                        {
                            m_spellUpgradeLevel[index].Value = level;
                        }

                        else
                        {
                            DataSlot ds = new DataSlot(cd, level);
                            m_spellUpgradeLevel.Add(ds);
                        }

                        break;
                    }

                    default:
                    {
                        int index = GetDataIndex(this.m_unitUpgradeLevel, cd);

                        if (index != -1)
                        {
                            m_unitUpgradeLevel[index].Value = level;
                        }

                        else
                        {
                            DataSlot ds = new DataSlot(cd, level);
                            m_unitUpgradeLevel.Add(ds);
                        }

                        break;
                    }
            }
        }
    }
}
