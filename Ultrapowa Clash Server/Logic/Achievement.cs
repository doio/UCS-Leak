namespace UCS.Logic
{
    internal class Achievement
    {
        const int m_vType = 0x015EF3C0;

        public Achievement()
        {
        }

        public Achievement(int index)
        {
            Index = index;
            Unlocked = false;
            Value = 0;
        }

        public int Id => m_vType + Index;

        public int Index { get; set; }
        public string Name { get; set; }
        public bool Unlocked { get; set; }
        public int Value { get; set; }
    }
}
